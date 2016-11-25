#!/usr/bin/env python
"""Sends temperature data to a Unioty server"""
import time
import unioty
from grove_i2c_temp_hum_hdc1000 import HDC1000

SERVER_IP = '192.168.0.2'
SERVER_PORT = 25556
DEV_ID = 0x01
HDC_CTRL_ID = 0x03

def main():
    """The main function"""
    hub = unioty.setup(DEV_ID, SERVER_IP, SERVER_PORT)

    hdc = HDC1000()
    hdc.Config()

    while True:
        print 'Temperature: %.2f C' % hdc.Temperature()
        hub.write_udp_float(HDC_CTRL_ID, hdc.Temperature())
        time.sleep(0.5)

if __name__ == "__main__":
    main()
