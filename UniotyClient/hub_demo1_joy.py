"""Roll a ball with joystick"""
#!/usr/bin/env python
import time
import grovepi
import unioty

# Connect the Grove Thumb Joystick to analog port A0

xPin = 0
yPin = 1
grovepi.pinMode(xPin,"INPUT")
grovepi.pinMode(yPin,"INPUT")

# Specifications
#     Min  Typ  Max  Click
#  X  206  516  798  1023
#  Y  203  507  797

SERVER_IP = '169.254.231.138'
SERVER_PORT = 25556
DEV_ID = 0x01
JOY_X_CTRL_ID = 0x03
JOY_Y_CTRL_ID = 0x04

JOY_X_BASE = 510
JOY_Y_BASE = 510
JOY_X_RANGE = (250, 750)
JOY_Y_RANGE = (260, 760)

def main():
    """The main function"""
    hub = unioty.setup_pusher(DEV_ID, SERVER_IP, SERVER_PORT)

    while True:
        try:
            # Get X/Y coordinates
            x_coord = grovepi.analogRead(xPin)
            y_coord = grovepi.analogRead(yPin)

            # Was a click detected on the X axis?
            click = 1 if x_coord >= 1020 else 0

            if click == 0:
                # Convert joy values to float between 0 and 1
                x_conv = float(x_coord - JOY_X_RANGE[0]) / (JOY_X_RANGE[1] - JOY_X_RANGE[0])
                x_conv = clamp(x_conv, 0.0, 1.0)
                y_conv = float(y_coord - JOY_Y_RANGE[0]) / (JOY_Y_RANGE[1] - JOY_Y_RANGE[0])
                y_conv = clamp(y_conv, 0.0, 1.0)
                hub.write_udp_float(JOY_X_CTRL_ID, x_conv)
                hub.write_udp_float(JOY_Y_CTRL_ID, y_conv)
                print("x =", x_conv, " y =", y_conv)
            else:
                hub.write_udp_float(JOY_X_CTRL_ID, 0.5)
                hub.write_udp_float(JOY_Y_CTRL_ID, 0.5)

            time.sleep(.1)

        except IOError:
            print "Error"

def clamp(number, minn, maxn):
    """Clamp a number between min and max values"""
    return max(min(maxn, number), minn)

if __name__ == "__main__":
    main()
