
\ CC2 - Development Environment
\ slothbear - BenzWorld.org
\ AVR - eForth1


 \\ VARIABLES ------------------------------------------------------------
variable MODE				\ Operating mode (set internally by functions)
variable PIN1				\ Simulated Vac Actuator
variable PIN2 				\ ^
variable PIN3				\ ^
variable PINC				\ On/Off valve coolant flow to heater core
variable PINR				\ Recirculate Door
variable BLOWERPIN			\ Level of blower (0,1,2) (5 is auto)
variable PINB1				\ Blower level - resistor ladder step
variable PINB2				\ Blower level - resistor ladder step
variable PINB3				\ Blower level - resistor ladder step
variable OAT				\ Outside Air Temp
variable IAT				\ Inside Air Temp
variable TEMPSET			\ Desired Air Temp (FOR AUTO)

\ Vents
: PIN1-H   1 2 OUT   1 PIN1 ! ;
: PIN1-L   0 2 OUT   0 PIN1 ! ;
: PIN2-H   1 3 OUT   1 PIN2 ! ;
: PIN2-L   0 3 OUT   0 PIN2 ! ;
: PIN3-H   1 4 OUT   1 PIN3 ! ;
: PIN3-L   0 4 OUT   0 PIN3 ! ;

\ Recirculate
: PINR-H   1 5 OUT   1 PINR ! ;
: PINR-L   0 5 OUT   0 PINR ! ;

\ Recirc LED
: RLED-ON    1 6 OUT ;
: RLED-OFF   0 6 OUT ;

\ Coolant Valve
: PINC-H   1 7 OUT   1 PINC ! ;
: PINC-L   0 7 OUT   0 PINC ! ;

\ Blower resistor taps
: PINB1-H   1 9 OUT   1 PINB1 ! ;
: PINB1-L   0 9 OUT   0 PINB1 ! ;
: PINB2-H   1 10 OUT  1 PINB2 ! ;
: PINB2-L   0 10 OUT  0 PINB2 ! ;
: PINB3-H   1 11 OUT  1 PINB3 ! ;
: PINB3-L   0 11 OUT  0 PINB3 ! ;


\\ PINS ------------------------------------------------------------
\ MODE is a variable that signifies which mode is currently operating eg. Defrost or Bi-Level
: listPin    	PIN1 @ . PIN2 @ . PIN3 @ .  s" H/C" type PINC @ .  s" MODE" type MODE @ . s" R" type PINR @ . BLOWERPIN @ . cr ;
: clearPin    	PIN1-L PIN2-L PIN3-L PINR-L PINC-L 0 MODE ! ;
: killBlower 	PINB1-L PINB2-L PINB3-L 0 BLOWERPIN ! ;

: setModePins  ( p1 p2 p3 -- )
    if PIN3-H else PIN3-L then
    if PIN2-H else PIN2-L then
    if PIN1-H else PIN1-L then ;

: checkMode    	cr MODE @ . cr  ;
: setMode   	( mode -- ) MODE !  ;
: outputError   cr s" CHECK OUTPUTS" type cr ;

\ D for display. Debugging commands to print the value of a placeholder variable
\ Last priority
: dOAT   		s" OAT: " OAT @ . ;
: dIAT   		s" IAT: " IAT @ . ;
: dTEMPSET   	s" Temperature Setting: " TEMPSET @ . ;

\ Simple door and valve functions
: RECIRC    PINR-H RLED-ON ;
: FRESH    	PINR-L RLED-OFF ;		
: HEAT    	PINC-H ;
: COOL 		PINC-L ;
: BLOW   	( num -- )  BLOWERPIN ! ;  \ Manually set blower speed (0,1,2,5)
: OFF   	clearPin killBlower ;

: DEFROST   
	0 0 0   setModePins					\ Set associated vac pattern for defrost mode (currently fake)
	PIN1 @ PIN2 @  + 2  =  IF			\ Ensure pins are proper (These are not fully fleshed out for all)
		cr s" DEFROST" type cr
		1 MODE ! 						\ Announce MODE1 in variable
		PINB3-H					\ Command High blower speed
		FRESH							\ Ensure recirculate is off
		ELSE
		outputError 					\ Something is wrong
		THEN ;
		
: LOW   
	 0 1 0  setModePins
	 PIN1 @ PIN2 @  + 1 =  IF
		cr s" LOW" type cr
		2 MODE ! 
		PINB1-H
		FRESH
		ELSE
		outputError 
		THEN ;
		
: HIGH 
	0 0 1  setModePins
	PIN1 @ PIN2 @  + 0  =  IF
		cr s" HIGH" type cr
		3 MODE ! 
		PINB2-H
		RECIRC
		ELSE
		outputError 
		THEN ;
	
: BILEVEL
	0 1 1  setModePins
	PIN1 @ PIN2 @  + 1  =  IF
		cr s" BI-LEVEL" type cr
		4 MODE ! 
		PINB2-H
		RECIRC
		ELSE
		outputError 
		THEN ;
		
\ : autoBlowerControl             NEED TO FIGURE THIS OUT. FAN CONTROL FOR AUTO CLIMATE
\ Last priority
\	IAT @ TEMPSET @ - abs
\ 		dup 100 > if drop 2 exit then 
\		dup 50 > if drop 1 exit then   
\		drop 0 ;     	

\ Last priority		
: AUTO
	5 MODE !
	cr s" AUTOMATIC" type cr
	\ autoBlowerControl BLOWERPIN !
	IAT @ TEMPSET @ > IF   
    		COOL     
  		ELSE
    		HEAT
   	 	THEN ;


\ DEBUG ===============================================

: RESET    clearPin 0 setMode heat recirc ;

: SELFTEST
	cr s" SELFTEST (expect delay) =====" type cr
		listPin clearPin listPin

	cr s" RECIRC" type cr
		recirc listPin 4000 ms fresh listPin 4000 ms

	cr s" SENSORS" type cr
		dOAT dIAT dTEMPSET cr

	cr s" MODES =================" type
		defrost 4000 ms listPin 
		high 4000 ms listPin
		auto 4000 ms listPin
		bilevel 4000 ms listPin
		low 4000 ms listPin

	cr s" TEMPERATURE" type cr
		heat 10000 ms listPin
		cool 10000 ms listPin
		0 0 0 setModePins 0 setMode
		killBlower
		heat recirc

	cr s" SELFTEST COMPLETE - VERIFY RESULTS" type cr ;
	
\ : pinMode ( pin mode -- ) ." pinMode " . . cr ;

 : S1 ( set -- )   dup PIN1 ! ." 1 " . cr ;
 : S2 ( set -- )   dup PIN2 ! ." 2 " . cr ;
 : S3 ( set -- )   dup PIN3 ! ." 3 " . cr ;
 : S5 ( set -- )   dup PINR ! ." 5 " . cr ;	
 : S6 ( set -- )   dup PINC ! ." 6 " . cr ;			 


\ STARTUP LOGIC ========================================
\ Just a reset to zero state. Heat on recirculate with no blower
: INIT   cr s" CLIMATE CONTROL INITIALIZED ---- " type 

1 2 PINMODE
1 3 PINMODE
1 4 PINMODE
1 5 PINMODE
1 6 PINMODE
1 7 PINMODE
1 9 PINMODE
1 10 PINMODE
1 11 PINMODE

0 0 0 setModePins
0 BLOWERPIN !
heat recirc
listPin
s" OAT " type dOAT s" IAT " type dIAT cr  ;

INIT

