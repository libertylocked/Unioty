package main

import (
	"log"
	"net"
)

// MsgAck is an acknowledgement message
var MsgAck = []byte{0x00}

func handleConn(c net.Conn) {
	defer c.Close()

	// Read 1 byte devID
	// 0x00 is reserved for master
	idBuf := make([]byte, 1)
	_, err := c.Read(idBuf)
	if err != nil {
		log.Println("Error reading ID", err)
		return
	}

	deviceID := idBuf[0]
	if deviceID == 0x00 {
		handleConnMaster(c)
	} else {
		handleConnDevice(c, deviceID)
	}
}
