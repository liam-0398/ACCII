#include <eForth1.h>

/**
 * CC2 - W123 Climate Control Replacement
 * slothbear - BenzWorld.org
 * AVR - eForth1
 *
 * Pin Assignments:
 *   2,3,4   - Vacuum solenoids (vents)
 *   5       - Recirculate solenoid
 *   6       - Recirculate LED
 *   7       - Coolant valve
 *   9,10,11 - Blower resistor taps
 */

PROGMEM const char code[] =

    // VARIABLES
    "VARIABLE MODE\n"
    "VARIABLE PIN1\n"
    "VARIABLE PIN2\n"
    "VARIABLE PIN3\n"
    "VARIABLE PINC\n"
    "VARIABLE PINR\n"
    "VARIABLE BLOWERPIN\n"
    "VARIABLE PINB1\n"
    "VARIABLE PINB2\n"
    "VARIABLE PINB3\n"
    "VARIABLE OAT\n"
    "VARIABLE IAT\n"
    "VARIABLE TEMPSET\n"

    // Vent solenoids
    ": PIN1-H 1 2 OUT 1 PIN1 ! ;\n"
    ": PIN1-L 0 2 OUT 0 PIN1 ! ;\n"
    ": PIN2-H 1 3 OUT 1 PIN2 ! ;\n"
    ": PIN2-L 0 3 OUT 0 PIN2 ! ;\n"
    ": PIN3-H 1 4 OUT 1 PIN3 ! ;\n"
    ": PIN3-L 0 4 OUT 0 PIN3 ! ;\n"

    // Recirculate solenoid
    ": PINR-H 1 5 OUT 1 PINR ! ;\n"
    ": PINR-L 0 5 OUT 0 PINR ! ;\n"

    // Recirc LED
    ": RLED-ON 1 6 OUT ;\n"
    ": RLED-OFF 0 6 OUT ;\n"

    // Coolant valve
    ": PINC-H 1 7 OUT 1 PINC ! ;\n"
    ": PINC-L 0 7 OUT 0 PINC ! ;\n"

    // Blower resistor taps
    ": PINB1-H 1 9 OUT 1 PINB1 ! ;\n"
    ": PINB1-L 0 9 OUT 0 PINB1 ! ;\n"
    ": PINB2-H 1 10 OUT 1 PINB2 ! ;\n"
    ": PINB2-L 0 10 OUT 0 PINB2 ! ;\n"
    ": PINB3-H 1 11 OUT 1 PINB3 ! ;\n"
    ": PINB3-L 0 11 OUT 0 PINB3 ! ;\n"

    // listPin split to stay under buffer limit
    ": lP1 PIN1 @ . PIN2 @ . PIN3 @ . ;\n"
    ": lP2 .\" H/C\" PINC @ . .\" M\" MODE @ . .\" R\" PINR @ . BLOWERPIN @ . CR ;\n"
    ": listPin lP1 lP2 ;\n"

    // Control
    ": clearPin PIN1-L PIN2-L PIN3-L PINR-L PINC-L 0 MODE ! ;\n"
    ": killBlower PINB1-L PINB2-L PINB3-L 0 BLOWERPIN ! ;\n"

    // setModePins split - each IF/THEN consumes one stack value
    ": sMP3 IF PIN3-H ELSE PIN3-L THEN ;\n"
    ": sMP2 IF PIN2-H ELSE PIN2-L THEN ;\n"
    ": sMP1 IF PIN1-H ELSE PIN1-L THEN ;\n"
    ": setModePins sMP3 sMP2 sMP1 ;\n"

    ": checkMode CR MODE @ . CR ;\n"
    ": setMode MODE ! ;\n"
    ": outputError CR .\" CHECK OUTPUTS\" CR ;\n"

    // Sensor debug
    ": dOAT .\" OAT:\" OAT @ . ;\n"
    ": dIAT .\" IAT:\" IAT @ . ;\n"
    ": dTEMPSET .\" SET:\" TEMPSET @ . ;\n"

    // Simple control words
    ": RECIRC PINR-H RLED-ON ;\n"
    ": FRESH PINR-L RLED-OFF ;\n"
    ": HEAT PINC-H ;\n"
    ": COOL PINC-L ;\n"
    ": BLOW BLOWERPIN ! ;\n"
    ": OFF clearPin killBlower ;\n"

    // Mode words split into setup + action helpers
    ": DF-SET 1 1 0 setModePins ;\n"
    ": DF-ACT CR .\" DEFROST\" CR 1 MODE ! PINB3-H FRESH ;\n"
    ": DEFROST DF-SET PIN1 @ PIN2 @ + 2 = IF DF-ACT ELSE outputError THEN ;\n"

    ": LO-SET 0 1 0 setModePins ;\n"
    ": LO-ACT CR .\" LOW\" CR 2 MODE ! PINB1-H FRESH ;\n"
    ": LO LO-SET PIN1 @ PIN2 @ + 1 = IF LO-ACT ELSE outputError THEN ;\n"

    ": HI-SET 0 0 1 setModePins ;\n"
    ": HI-ACT CR .\" HIGH\" CR 3 MODE ! PINB2-H RECIRC ;\n"
    ": HI HI-SET PIN1 @ PIN2 @ + 0 = IF HI-ACT ELSE outputError THEN ;\n"

    ": BL-SET 0 1 1 setModePins ;\n"
    ": BL-ACT CR .\" BI-LEVEL\" CR 4 MODE ! PINB2-H RECIRC ;\n"
    ": BILEVEL BL-SET PIN1 @ PIN2 @ + 1 = IF BL-ACT ELSE outputError THEN ;\n"

    ": AUTO 5 MODE ! CR .\" AUTO\" CR IAT @ TEMPSET @ > IF COOL ELSE HEAT THEN ;\n"

    // Debug
    ": RESET clearPin 0 setMode HEAT RECIRC ;\n"

    // SELFTEST broken into sub-words, each under 80 chars
    ": ST-HDR CR .\" SELFTEST\" CR listPin clearPin listPin ;\n"
    ": ST-REC CR .\" RECIRC\" CR RECIRC listPin 4000 DELAY FRESH listPin 4000 DELAY ;\n"
    ": ST-SEN CR .\" SENSORS\" CR dOAT CR dIAT CR dTEMPSET CR ;\n"
    ": ST-MD1 CR .\" MODES\" CR DEFROST 4000 DELAY listPin HI 4000 DELAY listPin ;\n"
    ": ST-MD2 AUTO 4000 DELAY listPin BILEVEL 4000 DELAY listPin ;\n"
    ": ST-MD3 LO 4000 DELAY listPin ;\n"
    ": ST-TMP CR .\" TEMP\" CR HEAT 4000 DELAY listPin COOL 4000 DELAY listPin ;\n"
    ": ST-CLN 0 0 0 setModePins 0 setMode killBlower HEAT RECIRC CR .\" DONE\" CR ;\n"
    ": SELFTEST ST-HDR ST-REC ST-SEN ST-MD1 ST-MD2 ST-MD3 ST-TMP ST-CLN ;\n"

    // Direct set debug words
    ": S1 DUP PIN1 ! .\" P1:\" . CR ;\n"
    ": S2 DUP PIN2 ! .\" P2:\" . CR ;\n"
    ": S3 DUP PIN3 ! .\" P3:\" . CR ;\n"
    ": S5 DUP PINR ! .\" PR:\" . CR ;\n"
    ": S6 DUP PINC ! .\" PC:\" . CR ;\n"

    // INIT split into sub-words
    ": PIN-A 1 2 PINMODE 1 3 PINMODE 1 4 PINMODE 1 5 PINMODE 1 6 PINMODE ;\n"
    ": PIN-B 1 7 PINMODE 1 9 PINMODE 1 10 PINMODE 1 11 PINMODE ;\n"
    ": IN-ST 0 0 0 setModePins 0 BLOWERPIN ! HEAT RECIRC ;\n"
    ": INIT CR .\" CC2 INIT\" CR PIN-A PIN-B IN-ST listPin ;\n"

    "INIT\n"
;

void setup() {
    Serial.begin(115200);
    while (!Serial);
    ef_setup(code);
}

void loop() {
    ef_run();
}
