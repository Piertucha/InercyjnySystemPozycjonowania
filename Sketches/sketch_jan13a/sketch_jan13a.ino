#include "MPU6050_6Axis_MotionApps20.h"
#include "Wire.h"

MPU6050 mpu;

Quaternion q;
VectorFloat gravity;
uint8_t fifoBuffer[64];

uint16_t packetSize;
uint16_t fifoCount;
float ypr[3];

void setup() {
  Serial.begin(115200);
  Wire.begin();
  TWBR = 24;
  mpu.initialize();
  mpu.dmpInitialize();

  // This offset value can be changed as needed
  mpu.setXAccelOffset(-1343);
  mpu.setYAccelOffset(-1155);
  mpu.setZAccelOffset(1033);
  mpu.setXGyroOffset(19);
  mpu.setYGyroOffset(-27);
  mpu.setZGyroOffset(16);
  mpu.setDMPEnabled(true);
  packetSize = mpu.dmpGetFIFOPacketSize();
  fifoCount = mpu.getFIFOCount();

  Serial.println("Stabilizing process ... Please wait ...");
  delay(18000);
  mpu.resetFIFO();
}

void loop() {
  while (fifoCount < packetSize) {
    fifoCount = mpu.getFIFOCount();
  }

  if (fifoCount == 1024) {

    mpu.resetFIFO();
    Serial.println(F("FIFO overflow!"));
  }
  else {

    if (fifoCount % packetSize != 0) {

      mpu.resetFIFO();
    }
    else {

      while (fifoCount >= packetSize) {
        mpu.getFIFOBytes(fifoBuffer, packetSize);
        fifoCount -= packetSize;
      }
      mpu.dmpGetQuaternion(&q, fifoBuffer);
      mpu.dmpGetGravity(&gravity, &q);
      mpu.dmpGetYawPitchRoll(ypr, &q, &gravity);

      Serial.print("ypr\t");
      Serial.print(ypr[0] * 180 / PI);
      Serial.print("\t");
      Serial.print(ypr[1] * 180 / PI);
      Serial.print("\t");
      Serial.print(ypr[2] * 180 / PI);
      Serial.println();
    }
  }
}
