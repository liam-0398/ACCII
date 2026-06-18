## Introduction

This is an AVRPascal based replacement for the ACCII climate control system. It is under heavy development and is currently in the prototype phase. 

A Pascal, FORTH and Lisp version are included. The FORTH and Lisp version will be used for hardware setup and validation and the Pascal version will be used in production as the final firmware for my specific variation.

All versions will have feature parity so if you want to use an ATMega or an ESP32 you are covered. This project is meant to be as open and flexible as possible while still maintaining a clear vision and a consistent outcome. 

Project can be followed here https://www.benzworld.org/threads/diy-acc2-unit-replacement-project-brainstorming.3170357/

## Planned Features

```
All factory vent mode control
Blower control
Temperature control via third party 12v solenoid
Automatic mode with temp sensing and intelligent defrost# Last Priority
```
## What is Not Planned

AC. I have none in my car. 

## Hardware

```
Arduino Nano or Standalone ATMega 328p or ESP32 Devkit
12v Buck Converter
3.3v MOSFETs
Buttons
LEDs
Rotary Encoder
2x Analog temp sensors # For automatic mode
```
