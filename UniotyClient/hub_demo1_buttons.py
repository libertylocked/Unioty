#!/usr/bin/env python
"""Controls the ball in Unioty demo 1"""
import time
import grovepi
import unioty

# Connect the Grove Button to port D3
# Connect the Grove Magnetic Switch to port D2
# SIG,NC,VCC,GND
BUTTON = 3
MAG_SWITCH = 2
grovepi.pinMode(BUTTON, "INPUT")
grovepi.pinMode(MAG_SWITCH, "INPUT")

SERVER_IP = '192.168.0.2'
SERVER_PORT = 25556
DEV_ID = 0x01
BUTTON_CTRL_ID = 0x01
MAG_SWITCH_CTRL_ID = 0x02

def main():
    """The main function"""
    hub = unioty.setup(DEV_ID, SERVER_IP, SERVER_PORT)
    print 'TCP connecting to', SERVER_IP, SERVER_PORT
    hub.connect_tcp()
    print 'TCP connected'

    button_state = 0
    button_state_prev = 0
    mag_switch_state = 0
    mag_switch_state_prev = 0

    while True:
        try:
            button_state = grovepi.digitalRead(BUTTON)
            mag_switch_state = grovepi.digitalRead(MAG_SWITCH)

            # Assert valid button state
            if button_state == 0 or button_state == 1:
                # Only send over TCP when state changes
                if button_state != button_state_prev:
                    print 'Button', button_state
                    hub.write_tcp_byte(BUTTON_CTRL_ID, button_state)
                button_state_prev = button_state

            if mag_switch_state == 0 or mag_switch_state == 1:
                if mag_switch_state != mag_switch_state_prev:
                    print 'Mag Switch', mag_switch_state
                    hub.write_tcp_byte(MAG_SWITCH_CTRL_ID, mag_switch_state)
                mag_switch_state_prev = mag_switch_state

            time.sleep(0.01)

        except IOError as ex:
            print str(ex)

if __name__ == "__main__":
    main()
