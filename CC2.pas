program CC2_Arduino;
{W123 ACC2 Replacement Arduino}
{slothbear - BenzWorld}

uses
     fix16, hardwareserial, timer, digital;

const
  VACPIN1 = 2;
  VACPIN2 = 3;
  VACPIN3 = 4;
  VACPIN4 = 5;
  HEATPIN = 6;
  RECIRCP = 7;
  BLOWER  = 9;
  TMP01   = 10;
  TMP02   = 11; // placeholder, fix later

var
  vac1, vac2, vac3, vac4 : Integer;
  heat, recirc            : Integer;
  blowerSpeed             : Integer;
  targetTemp              : Integer;

procedure pinModes(v1, v2, v3, v4, h, r: Integer);
     begin
          vac1 := v1;  vac2 := v2;  vac3 := v3;  vac4 := v4;
          heat := h;   recirc := r;
          digitalWrite(VACPIN1, v1);
          digitalWrite(VACPIN2, v2);
          digitalWrite(VACPIN3, v3);
          digitalWrite(VACPIN4, v4);
          digitalWrite(HEATPIN, h);
          digitalWrite(RECIRCP, r);
     end;

// PRINT CURRENT STATE OF PINS// PRINT CURRENT STATE OF PINS

procedure listPin;
     begin

          Serial.WriteChar(Char(pin1 + 48));
          Serial.Write(Char(pin2 + 48));
          Serial.Write(Char(pin3 + 48));
          Serial.Write(Char(pin4 + 48));
          Serial.Write(' R');
          Serial.Write(Char(pinR + 48));
          Serial.Write(' H');
          Serial.Write(Char(pinH + 48));
          Serial.Write(#13#10);

     end;

// RESET PINS TO ZERO
procedure pinReset;
     begin
          pinModes(0, 0, 0, 0, 0, 0);
     end;

procedure toggleRecirc;
     begin
          If pinR = 0 Then
               pinR := 1
             else
               pinR := 0;
     end;

procedure toggleHeat;
     begin
             pinH := 1;
     end;

procedure toggleCool;
     begin
             pinH := 0;
     end;

// ACTUAL CLIMATE CONTROL MODES
// CURRENTLY USING ARBITRARY PIN ASSIGNMENTS
procedure Low;
     begin
          pinModes(1, 0, 0, 0, 0, 0);
     end;

procedure High;
     begin
          pinModes(1, 1, 0, 0, 1, 0);
     end;

procedure BiLevel;
     begin
          pinModes(0, 0, 1, 1, 1, 0);
     end;

procedure Defrost;
     begin
          pinModes(1, 1, 1, 1, 0, 0);
     end;

// WIP - LATER PRIORITY
procedure Automatic;
     begin
          If tempsensor1 > tempselect then
             toggleCool;
          If tempsensor1 < tempselect then
             toggleHeat;
     end;

// GENERIC TESTING FUNCTION
procedure selfTest;
     begin
          Low;
          delay(5550);
          High;
          delay(5550);
          BiLevel;
          delay(5550);
          Defrost;
          delay(5550);
          toggleRecirc;
          delay(5550);
          toggleHeat;
          delay(5550);
          Serial.WriteLn('TEST COMPLETE');
     end;

// INITITALIZE SERIAL CONNECTION - 9600 BAUD
procedure serialInit;
     begin
           Serial.Start(9600);
           delay(200);
           Serial.WriteLn('SERIAL INIT COMPLETE');
     end;

// SCAN FOR SERIRAL INPUT AND SELECT MODE
// WILL BE DONE WITH BUTTONS WHEN IN THE CAR
begin
  pinReset;
  serialInit;
  listPin;

  Serial.WriteLn('W123 CC2');
  Serial.WriteLn('1 - Low');
  Serial.WriteLn('2 - High');
  Serial.WriteLn('3 - Bi-Level');
  Serial.WriteLn('4 - Defrost');
  Serial.WriteLn('5 - Recirculate');
  Serial.WriteLn('6 - Heat/Cool');
  Serial.WriteLn('7 - List Pins');
  Serial.WriteLn('8 - Selftest');

  repeat
    while Serial.Available = 0 do ;

    con := Serial.ReadByte;

    if (con >= 48) and (con <= 57) then
      con := con - 48;

    case con of
      1: Low;
      2: High;
      3: BiLevel;
      4: Defrost;
      5: toggleRecirc;
      6: toggleHeat;
      7: listPin;
      8: selfTest;
      9: ;

    else
      if (con <> (10-48)) and (con <> (13-48)) then
        Serial.WriteLn('Invalid Choice');
    end;

    delay(10);
    while Serial.Available > 0 do Serial.ReadByte;

  until con = 9;
end.
