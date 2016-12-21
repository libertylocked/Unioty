#!/usr/bin/env python
"""Press button to make the ball jump in Unioty demo 1"""
import time
import grovepi
import unioty

# Connect the Grove Button to port D3
# SIG,NC,VCC,GND
BUTTON = 3
grovepi.pinMode(BUTTON, "INPUT")

SERVER_IP = '192.168.0.2'
SERVER_PORT = 25556
DEV_ID = 0x01
BUTTON_CTRL_ID = 0x01

def main():
    """The main function"""
    hub = unioty.setup_pusher(DEV_ID, SERVER_IP, SERVER_PORT)
    print 'TCP connecting to', SERVER_IP, SERVER_PORT
    hub.connect_tcp()
    print 'TCP connected'

    button_state = 0
    button_state_prev = 0

    while True:
        try:
            button_state = grovepi.digitalRead(BUTTON)

            # Assert valid button state
            if button_state == 0 or button_state == 1:
                # Only send over TCP when state changes
                if button_state != button_state_prev:
                    print 'Button', button_state
                    hub.write_tcp_byte(BUTTON_CTRL_ID, button_state)
                button_state_prev = button_state

            time.sleep(0.01)

        except IOError as ex:
            print str(ex)

if __name__ == "__main__":
    main()
