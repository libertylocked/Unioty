package main

import (
	"log"
	"net"
)

func handleConnDevice(c net.Conn, deviceID byte) {
	log.Println("DEV:", deviceID, "connected from", c.RemoteAddr())
	// Make a store for this device
	devStore := controlCache.CreateStore(deviceID)
	defer controlCache.DeleteStore(deviceID)

	// Device writes 2 bytes a time
	// [ctrlID, state]
	buf := make([]byte, 2)

	for {
		reqLen, err := c.Read(buf)
		if err != nil {
			log.Println("DEV: Error reading!", err)
			break
		}
		if reqLen != 2 {
			log.Println("DEV: Unexpected data length!", reqLen)
			break
		}

		// Send ACK back
		c.Write(MsgAck)

		devStore.Set(buf[0], buf[1])
	}
}
