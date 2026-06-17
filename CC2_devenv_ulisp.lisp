; CC2 - Development Environment
; slothbear - BenzWorld.org
; ESP32 - uLisp
; WORK IN PROGRESS

(defvar VACPIN1 2)
(defvar VACPIN2 3)
(defvar VACPIN3 4)

(defvar RECIRCP 5)
(defvar RECIRCL 6)
(defvar COOLANTP 7)

(defvar BLOWER1 8)
(defvar BLOWER2 9)
(defvar BLOWER3 10)


(defun heat ()
    (digialwrite COOLANTP t))
(defun cool ()
    (digialwrite COOLANTP nil))

(defun blower ())


(defun hi ()
    (digialwrite VACPIN1 t)
    (digialwrite VACPIN2 t)
    (digialwrite VACPIN3 t))

(defun lo ()
    (digialwrite VACPIN1 t)
    (digialwrite VACPIN2 t)
    (digialwrite VACPIN3 t))

(defun bi-level ()
    (digialwrite VACPIN1 t)
    (digialwrite VACPIN2 t)
    (digialwrite VACPIN3 t))

(defun auto ()
    (digialwrite VACPIN1 t)
    (digialwrite VACPIN2 t)
    (digialwrite VACPIN3 t))

(defun self-test ()
    (hi)
    (lo)
    (bi-level)
    (auto))

(defun init ()
    (pinmode 2 t)
    (pinmode 3 t)
    (pinmode 4 t)
    (pinmode 5 t)
    (pinmode 6 t)
    (pinmode 7 t)
    (pinmode 8 t)
    (pinmode 9 t)
    (pinmode 9 t))

(init)