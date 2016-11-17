#!/usr/bin/env python
import time
import grovepi
import socket
import struct

# Connect the Grove Button to digital port D3
# SIG,NC,VCC,GND
button = 3
grovepi.pinMode(button,"INPUT")

hub_server = ('192.168.0.2', 25556)
hub_dev_id = 0x01
hub_control_id = 0x01

# Connect to hub server
print 'Connecting to server', hub_server
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect(hub_server)
dev_id_msg = bytearray()
dev_id_msg.append(hub_dev_id)
s.send(dev_id_msg)

dev_state_msg = bytearray()
dev_state_msg.append(hub_control_id)
dev_state_msg.append(0x00) # Arbitary initial value

# Loop, read button state, send to server
while True:
    try:
	state = grovepi.digitalRead(button)
	# XXX: Sometimes it reads 255 for unknown reasons
	if state != 0 and state != 1:
		continue

	stateByte = struct.pack('B', state)
        print(state)
	dev_state_msg[1] = stateByte
	s.sendall(dev_state_msg)
	# Wait until server sends an ack (1 byte)
	s.recv(1)
	#time.sleep(.015)

    except IOError:
        print ("Error")
