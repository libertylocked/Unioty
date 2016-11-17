package main

import (
	"log"
	"net"
)

func handleConnMaster(c net.Conn) {
	log.Println("MAS: connected from", c.RemoteAddr())

	// Master writes 2 bytes a time
	// [devID, ctrlID]
	buf := make([]byte, 2)

	for {
		reqLen, err := c.Read(buf)
		if err != nil {
			log.Println("MAS: Error reading!", err)
			break
		}
		if reqLen != 2 {
			log.Println("MAS: Unexpected data length in read!", reqLen)
			break
		}

		// Get the ctrl state for the ctrlID of devID
		state := controlCache.GetCache(buf[0], buf[1])
		// Write [devID, ctrlID, ctrlState]
		_, err = c.Write([]byte{buf[0], buf[1], state})
		if err != nil {
			log.Println("MAS: Error sending", state, err)
			return
		}
	}
}
