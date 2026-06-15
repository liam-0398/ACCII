
\ CC2 - Development Environment
\ slothbear - BenzWorld.org
\ AVR - FlashForth


 \\ VARIABLES ------------------------------------------------------------
variable MODE			\ Operating mode (set internally by functions)
variable PIN1				\ Simulated Vac Actuator
variable PIN2 				\ ^
variable PIN3				\ ^
variable PIN4				\ ^
variable PINC			\ On/Off valve coolant flow to heater core
variable PINR				\ Recirculate Door
variable BLOWERPIN		\ Level of blower (0,1,2) (5 is auto)

variable OAT				\ Outside Air Temp
variable IAT				\ Inside Air Temp
variable TEMPSET		\ Desired Air Temp (FOR AUTO)

\ Vents
: PIN1-H 12 1 digitalWrite 1 PIN1 ! ;
: PIN1-L 12 0 digitalWrite 0 PIN1 ! ;
: PIN2-H 12 1 digitalWrite 1 PIN2 ! ;
: PIN2-L 12 0 digitalWrite 0 PIN2 ! ;
: PIN3-H 12 1 digitalWrite 1 PIN3 ! ;
: PIN3-L 12 0 digitalWrite 0 PIN3 ! ;

\ Recirculate
: PINR-H 12 1 digitalWrite 1 PINR ! ;
: PINR-L 12 0 digitalWrite 0 PINR ! ;

\ Coolant Valve
: PINC-H 12 1 digitalWrite 1 PINC ! ;
: PINC-L 12 0 digitalWrite 0 PINC ! ;


 \\ PINS ------------------------------------------------------------
\ MODE is a variable that signifies which mode is currently operating eg Defrost or Bi-Level
: listPin    PIN1 @ . PIN2 @ . PIN3 @ . PIN4 @ . s" H/C" type PINC @ .  s" MODE" type MODE @ . s" R" type PINR @ . BLOWERPIN @ . cr ;
: clearPin    PIN1-L PIN2-L PIN3-L PINR-L PINC-L 0 MODE ! ;
: killBlower 0 BLOWERPIN ! ;
: setModePins   //PIN4 ! PIN3 ! PIN2 ! PIN1 ! 
	// If 0 PINX-L IF 1 PINX-H
;
: checkMode    cr MODE @ . cr  ;
: setMode   ( mode -- ) MODE !  ;
: outputError    cr s" CHECK OUTPUTS" type cr ;

\ D for display. Debugging commands to print the value of a placeholder variable
: dOAT   s" OAT: " OAT @ . ;
: dIAT   s" IAT: " IAT @ . ;
: dTEMPSET    s" Temperature Setting: " TEMPSET @ . ;

\ Simple door and valve functions
: RECIRC    PINR-H ;
: FRESH    	PINR-L ;		
: HEAT    	PINC-H ;
: COOL 		PINC-L ;
: BLOW   	( num -- )  BLOWERPIN ! ;  \ Manually set blower speed (0,1,2,5)
: OFF   	clearPin killBlower ;

: DEFROST   
	1 1 0 0  setModePins				\ Set associated vac pattern for defrost mode (currently fake)
	PIN1 @ PIN2 @  + 2  =  IF			\ Ensure pins are proper (These are not fully fleshed out for all)
		cr s" DEFROST" type cr
		1 MODE ! 						\ Announce MODE1 in variable
		2 BLOWERPIN !				\ Command High blower speed
		FRESH							\ Ensure recirculate is off
		ELSE
		outputError 					\ Something is wrong
		THEN ;
		
: LOW   
	 0 1 0 1 setModePins
	 PIN1 @ PIN2 @  + 1 =  IF
		cr s" LOW BLOWER" type cr
		2 MODE ! 
		1 BLOWERPIN !
		FRESH
		ELSE
		outputError 
		THEN ;
		
: HIGH 
	0 0 1 1 setModePins
	PIN1 @ PIN2 @  + 0  =  IF
		cr s" HIGH BLOWER" type cr
		3 MODE ! 
		2 BLOWERPIN !
		RECIRC
		ELSE
		outputError 
		THEN ;
	
: BILEVEL
	0 1 1 1 setModePins
	PIN1 @ PIN2 @  + 1  =  IF
		cr s" BI-LEVEL" type cr
		4 MODE ! 
		1 BLOWERPIN !
		RECIRC
		ELSE
		outputError 
		THEN ;
		
\ : autoBlowerControl             NEED TO FIGURE THIS OUT. FAN CONTROL FOR AUTO CLIMATE
\	IAT @ TEMPSET @ - abs
\ 		dup 100 > if drop 2 exit then 
\		dup 50 > if drop 1 exit then   
\		drop 0 ;     	
		
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
		0 0 0 0 setModePins 0 setMode
		killBlower
		heat recirc
	cr s" SELFTEST COMPLETE - VERIFY RESULTS" type cr ;
	
\ : pinMode ( pin mode -- ) ." pinMode " . . cr ;
\ : dWrite ( pin state -- ) cr ." PIN W " dup  .  cr ;
\ : dRead ( pin -- state ) cr ." PIN R " dup . 0 ;
\ : delay ms ;

 : S1 ( set -- )   dup PIN1 ! ." 1 " . cr ;
 : S2 ( set -- )   dup PIN2 ! ." 2 " . cr ;
 : S3 ( set -- )   dup PIN3 ! ." 3 " . cr ;
 : S4 ( set -- )   dup PIN4 ! ." 4 " . cr ;
 : S5 ( set -- )   dup PIN5CC ! ." 5 " . cr ;	
 : S6 ( set -- )   dup PIN6 ! ." 6 " . cr ;			 


\ STARTUP LOGIC ========================================
\ Just a reset to zero state. Heat on recirculate with no blower
: INIT   cr s" CLIMATE CONTROL INITIALIZED ---- " type 
0 0 0 0 setModePins
0 BLOWERPIN !
heat recirc
listPin
s" OAT " type dOAT s" IAT " type dIAT cr  ;

INIT

