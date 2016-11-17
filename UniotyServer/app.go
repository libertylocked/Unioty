package main

import (
	"log"
	"net"
	"os"
)

func main() {
	port := "25556"

	listener, err := net.Listen("tcp", ":"+port)
	if err != nil {
		log.Println(err)
		os.Exit(1)
	}

	defer listener.Close()
	log.Println("Listening on port", port)

	for {
		// Listen for an incoming connection.
		conn, err := listener.Accept()
		if err != nil {
			log.Println("Error accepting: ", err.Error())
			os.Exit(1)
		}
		// Handle connections in a new goroutine.
		go handleConn(conn)
	}
}
