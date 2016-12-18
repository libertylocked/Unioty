#!/usr/bin/env python
"""Lights up LED when ball is over 4m high in Unioty demo 1"""
import time
import grovepi
import unioty

# Connect first LED in Chainable RGB LED chain to digital port D7
# In: CI,DI,VCC,GND
# Out: CO,DO,VCC,GND
LED = 7
numleds = 1
grovepi.pinMode(LED,"OUTPUT")
colorBlack = 0

SERVER_IP = '192.168.0.2'
SERVER_PORT = 25556
HOST_HEIGHT_CTRL_ID = 0x00

def main():
    """The main function"""
    # Turn off LED at start
    grovepi.chainableRgbLed_init(LED, numleds)
    grovepi.chainableRgbLed_test(LED, numleds, colorBlack)

    poller = unioty.setup_poller(SERVER_IP, SERVER_PORT)
    print 'TCP polling from', SERVER_IP, SERVER_PORT
    poller.read_host_control(HOST_HEIGHT_CTRL_ID, on_off_led)

def on_off_led(data):
    """Turn on or off LED based on data"""
    print data[0]
    if data[0] == 1:
        # turn on LED
        grovepi.storeColor(255, 95, 74)
        grovepi.chainableRgbLed_pattern(LED, 0, 0)
    else:
        # turn off LED
        grovepi.chainableRgbLed_test(LED, 1, colorBlack)

if __name__ == "__main__":
    main()
