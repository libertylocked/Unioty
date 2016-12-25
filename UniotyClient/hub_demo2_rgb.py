#!/usr/bin/env python
"""Syncs the RGB LED on the Pi with the RGB light in the game"""
import time
import grovepi
import unioty

# Connect first LED in Chainable RGB LED chain to digital port D7
# In: CI,DI,VCC,GND
# Out: CO,DO,VCC,GND
LED = 7
grovepi.pinMode(LED, "OUTPUT")
COLOR_BLACK = 0

SERVER_IP = '192.168.0.2'
SERVER_PORT = 25556
HOST_CONTROL_ID = 0x01

def main():
    """The main function"""
    # Turn off LED at start
    grovepi.chainableRgbLed_init(LED, 1)
    grovepi.chainableRgbLed_test(LED, 1, COLOR_BLACK)

    poller = unioty.setup_poller(SERVER_IP, SERVER_PORT)
    print 'TCP polling from', SERVER_IP, SERVER_PORT
    poller.read_host_control(HOST_CONTROL_ID, change_color)

def change_color(data):
    """Change the color of the LED"""
    color_r = data[0]
    color_g = data[1]
    color_b = data[2]
    print color_r, color_g, color_b

    grovepi.storeColor(color_r, color_g, color_b)
    grovepi.chainableRgbLed_pattern(LED, 0, 0)

if __name__ == "__main__":
    main()
