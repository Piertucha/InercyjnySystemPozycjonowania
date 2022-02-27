
#include <ESP8266WiFi.h>
#include <WiFiUdp.h>
#include "Wire.h" // 

const char* ssid = "SSID"; //SSID Sieci
const char* password = "password";  // Hasło Sieci
const String ip = "192.168.0.101"; // IP odbiornika
char Buf[50];
WiFiUDP Udp;
unsigned int localUdpPort = 4210;  // local port to listen on

const int MPU_ADDR = 0x68; 

int16_t raw_acc_x, raw_acc_y, raw_acc_z; 
int16_t raw_gyro_x, raw_gyro_y, raw_gyro_z; 
int16_t temperature; 
float d_gyro_x, d_gyro_y, d_gyro_z, th_gyro_x, th_gyro_y, th_gyro_z, norm_acc_x, norm_acc_y, norm_acc_z, norm_gyro_x, norm_gyro_y, norm_gyro_z,acc_pitch, acc_roll, kom_angle_x, kom_angle_y, bez_angle_x=0, bez_angle_y=0, bez_angle_z=0;
float pdt, dt;
double PP[2][2],PR[2][2], KP[2], KR[2];
double SP, SR, yP, yR;
double KP_rate, KR_rate;
String s;

double QP_angle=0.001;
double QP_bias = 0.003;
double RP_measure = 0.03;

double KP_angle = 0;
double KP_bias = 0;

double QR_angle=0.001;
double QR_bias = 0.003;
double RR_measure = 0.03;

double KR_angle = 0;
double KR_bias = 0;

void setup() {
  Serial.begin(9600);
  Wire.begin();
 d_gyro_x=0;
 d_gyro_y=0;
 d_gyro_z=0;
 th_gyro_x=0;
 th_gyro_y=0;
 th_gyro_z=0;
PP[0][0]=0;
PP[0][1]=0;
PP[1][0]=0;
PP[1][1]=0;

PR[0][0]=0;
PR[0][1]=0;
PR[1][0]=0;
PR[1][1]=0;

  Wire.beginTransmission(MPU_ADDR); 
  Wire.write(0x6B); 
  Wire.write(0); 
  Wire.endTransmission(true);
  Serial.println();


//Kalibrancja żyroskopu
    float sumX = 0;
    float sumY = 0;
    float sumZ = 0;
    float sigmaX = 0;
    float sigmaY = 0;
    float sigmaZ = 0;

    for(int i=0; i <3000; i++){
      Wire.beginTransmission(MPU_ADDR);
      Wire.write(0x43);
      Wire.endTransmission(false);
      Wire.requestFrom(MPU_ADDR,6,true);
      raw_gyro_x = Wire.read()<<8 | Wire.read(); // reading registers: 0x43 (GYRO_XOUT_H) and 0x44 (GYRO_XOUT_L)
      raw_gyro_y = Wire.read()<<8 | Wire.read(); // reading registers: 0x45 (GYRO_YOUT_H) and 0x46 (GYRO_YOUT_L)
      raw_gyro_z = Wire.read()<<8 | Wire.read(); // reading registers: 0x47 (GYRO_ZOUT_H) and 0x48 (GYRO_ZOUT_L)

      sumX+=(float)raw_gyro_x;
      sumY+=(float)raw_gyro_y;
      sumZ+=(float)raw_gyro_z;

      sigmaX+=(float)raw_gyro_x*(float)raw_gyro_x;
      sigmaY+=(float)raw_gyro_y*(float)raw_gyro_y;
      sigmaZ+=(float)raw_gyro_z*(float)raw_gyro_z;
      
      delay(5);
      }

d_gyro_x=sumX / 3000;
d_gyro_y=sumY / 3000;
d_gyro_z=sumZ / 3000;

th_gyro_x=sqrt((sigmaX / 50) - (d_gyro_x * d_gyro_x));
th_gyro_y=sqrt((sigmaY / 50) - (d_gyro_y * d_gyro_y));
th_gyro_z=sqrt((sigmaZ / 50) - (d_gyro_z * d_gyro_z));

String x =String(d_gyro_x)+" "+String(d_gyro_y)+" "+String(d_gyro_z)+" "+ String(th_gyro_x)+" "+ String(th_gyro_y)+" "+String(th_gyro_z);
Serial.println(x);
//Łączenie z siecią
  Serial.printf("Connecting to %s ", ssid);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print(".");
  }
  Serial.println(" connected");

  Udp.begin(localUdpPort);
  Serial.printf("Now listening at IP %s, UDP port %d\n", WiFi.localIP().toString().c_str(), localUdpPort);

  pdt=millis();
}
void loop() {
  //Odczyt surowych danych z MPU6050
  Wire.beginTransmission(MPU_ADDR);
  Wire.write(0x3B);
  Wire.endTransmission(false);
  Wire.requestFrom(MPU_ADDR, 7*2, true);
  
  // "Wire.read()<<8 | Wire.read();" means two registers are read and stored in the same variable
  raw_acc_x = Wire.read()<<8 | Wire.read(); // reading registers: 0x3B (ACCEL_XOUT_H) and 0x3C (ACCEL_XOUT_L)
  raw_acc_y = Wire.read()<<8 | Wire.read(); // reading registers: 0x3D (ACCEL_YOUT_H) and 0x3E (ACCEL_YOUT_L)
  raw_acc_z = Wire.read()<<8 | Wire.read(); // reading registers: 0x3F (ACCEL_ZOUT_H) and 0x40 (ACCEL_ZOUT_L)
  temperature = Wire.read()<<8 | Wire.read(); // reading registers: 0x41 (TEMP_OUT_H) and 0x42 (TEMP_OUT_L)
  raw_gyro_x = Wire.read()<<8 | Wire.read(); // reading registers: 0x43 (GYRO_XOUT_H) and 0x44 (GYRO_XOUT_L)
  raw_gyro_y = Wire.read()<<8 | Wire.read(); // reading registers: 0x45 (GYRO_YOUT_H) and 0x46 (GYRO_YOUT_L)
  raw_gyro_z = Wire.read()<<8 | Wire.read(); // reading registers: 0x47 (GYRO_ZOUT_H) and 0x48 (GYRO_ZOUT_L)

  //Normalizacja wartości
  norm_acc_x=(float)raw_acc_x * 0.000061f *9.80665f;
  norm_acc_y=(float)raw_acc_y * 0.000061f *9.80665f;
  norm_acc_z=(float)raw_acc_z * 0.000061f *9.80665f ;

  norm_gyro_x=((float)raw_gyro_x - d_gyro_x)*0.015267f;
  norm_gyro_y=((float)raw_gyro_y - d_gyro_y)*0.015267f;
  norm_gyro_z=((float)raw_gyro_z - d_gyro_z)*0.015267f;


  //Kalkulacja nachylenia i obrotu
  acc_pitch = -(atan2(norm_acc_x, sqrt(norm_acc_y*norm_acc_y + norm_acc_z * norm_acc_z))*180.0/PI);
  acc_roll = (atan2(norm_acc_y, norm_acc_z))*180.0/PI;

dt=(millis()-pdt)*0.001;
  //Kalkulacja kątów X i Y metodą filtru komplementarnego
  kom_angle_x=(0.98f*(norm_acc_x + norm_gyro_x*dt))+(0.98f*acc_roll);
  kom_angle_y=(0.98f*(norm_acc_y + norm_gyro_y*dt))+(0.98f*acc_pitch);

  //Kalkulacja kątów X Y Z bez filtrów
  bez_angle_x=bez_angle_x + norm_gyro_x*dt;
  bez_angle_y=bez_angle_y + norm_gyro_y*dt;
  //bez_angle_z=(bez_angle_z + norm_gyro_z*dt)/2.0f;
  bez_angle_z=bez_angle_z + norm_gyro_z*dt;


 

  //Kalkulacja Filtrem Kalmana
//kalPitch

KP_rate = norm_gyro_x - KP_bias;
KP_angle = KP_angle + dt * KP_rate;

    PP[0][0] = PP[0][0] + dt * (PP[1][1] + PP[0][1]) + QP_angle * dt;
    PP[0][1] = PP[0][1] - dt * PP[1][1];
    PP[1][0] = PP[1][0] - dt * PP[1][1];
    PP[1][1] = PP[1][1] + QP_bias * dt;

  SP= PP[0][0]+ RP_measure;
  KP[0] = PP[0][0] / SP;
  KP[1] = PP[1][0] / SP;
  yP= acc_pitch - KP_angle;

  KP_angle = KP_angle + KP[0] * yP;
  KP_bias = KP_bias + KP[1] * yP;
    PP[0][0] = PP[0][0]- KP[0] * PP[0][0];
    PP[0][1] = PP[0][1] - KP[0] * PP[0][1];
    PP[1][0] = PP[1][0] - KP[1] * PP[0][0];
    PP[1][1] = PP[1][1] - KP[1] * PP[0][1];



    //kalRoll

KR_rate = norm_gyro_y - KR_bias;
KR_angle = KR_angle + dt * KR_rate;

    PR[0][0] = PR[0][0] +dt * (PR[1][1] + PR[0][1]) + QR_angle * dt;
    PR[0][1] = PR[0][1] - dt * PR[1][1];
    PR[1][0] = PR[1][0] - dt * PR[1][1];
    PR[1][1] = PR[1][1] + QR_bias * dt;

  SR= PR[0][0]+ RR_measure;
  KR[0] = PR[0][0] / SR;
  KR[1] = PR[1][0] / SR;
  yR= acc_roll - KR_angle;

  KR_angle = KR_angle + KR[0] * yR;
  KR_bias = KR_bias + KR[1] * yR;
    PR[0][0] = PR[0][0] - KR[0] * PR[0][0];
    PR[0][1] = PR[0][1] - KR[0] * PR[0][1];
    PR[1][0] = PR[1][0] - KR[1] * PR[0][0];
    PR[1][1] = PR[1][1] - KR[1] * PR[0][1];
       pdt=millis();
  // print out data - odkomentować linijkę string s = ...

   // Wypisywanie z filtrem komplementarnym
String s =String(kom_angle_x)+" "+String(kom_angle_y)+" "+String(bez_angle_z)+" "+ String(norm_acc_x)+" "+ String(norm_acc_y)+" "+String(norm_acc_z);
  // Wypisywanie z filtrem kalmana
  //String s =String(KP_angle)+" "+String(KR_angle)+" "+String(bez_angle_z)+" "+ String(norm_acc_x)+" "+ String(norm_acc_y)+" "+String(norm_acc_z);
   // Wypisywanie danych bez filtrowania
 //String s =String(bez_angle_x)+" "+String(bez_angle_y)+" "+String(bez_angle_z)+" "+ String(norm_acc_x)+" "+ String(norm_acc_y)+" "+String(norm_acc_z);
Serial.println(s);
 s.toCharArray(Buf,50);

  Udp.beginPacket("192.168.0.101",54687);
  Udp.write(Buf);
  Udp.endPacket();

}
