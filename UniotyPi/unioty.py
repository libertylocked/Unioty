"""A library for communication with a Unioty server from a Unioty device"""
import socket
import struct


class UniotyHub(object):
    """UniotyHub is a hub that communicates with a UniotyServer"""

    def __init__(self, device_id, server_ip, server_port):
        self.device_id = device_id
        self.server_address = (server_ip, server_port)
        self.sock_udp = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sock_tcp = None

    def __write_udp(self, control_id, data_type, data):
        message = bytearray()
        message.append(self.device_id)
        # control_id is a byte
        message.append(control_id)
        # data_type is a char
        message.append(data_type)
        # data is packed into bytes
        if data_type == 's' or data_type == 'r':
            message.extend(data)
        else:
            # Pack into little endian
            message.extend(struct.pack('<'+data_type, data))
        self.sock_udp.sendto(message, self.server_address)

    def __write_tcp(self, control_id, data_type, data):
        message = bytearray()
        message.append(control_id)
        message.append(data_type)
        if data_type == 's' or data_type == 'r':
            message.append(len(data))
            message.extend(data)
        else:
            packed = struct.pack('<'+data_type, data)
            message.append(len(packed))
            message.extend(packed)
        self.sock_tcp.sendall(message)


    def write_tcp_float(self, control_id, data):
        """Sends a float32 to server through TCP"""
        self.__write_tcp(control_id, 'f', data)

    def write_tcp_int(self, control_id, data):
        """Sends an int32 to server through TCP"""
        self.__write_tcp(control_id, 'i', data)

    def write_tcp_string(self, control_id, data):
        """Sends a string to server through TCP"""
        self.__write_tcp(control_id, 's', data)

    def write_tcp_byte(self, control_id, data):
        """Sends an unsigned byte to server through TCP"""
        self.__write_tcp(control_id, 'B', data)

    def write_tcp_raw(self, control_id, data):
        """Sends a byte array to server through TCP"""
        self.__write_tcp(control_id, 'r', data)

    def connect_tcp(self):
        """Set up TCP connection to the server"""
        if self.sock_tcp != None:
            self.sock_tcp.close()
        self.sock_tcp = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        intro_message = bytearray()
        intro_message.append(self.device_id)
        self.sock_tcp.connect(self.server_address)
        self.sock_tcp.sendall(intro_message)

    def write_udp_float(self, control_id, data):
        """Sends a float32 to server through UDP"""
        self.__write_udp(control_id, 'f', data)

    def write_udp_int(self, control_id, data):
        """Sends an int32 to server through UDP"""
        self.__write_udp(control_id, 'i', data)

    def write_udp_string(self, control_id, data):
        """Sends a string to server through UDP"""
        self.__write_udp(control_id, 's', data)

    def write_udp_byte(self, control_id, data):
        """Sends an unsigned byte to server through UDP"""
        self.__write_udp(control_id, 'B', data)

    def write_udp_raw(self, control_id, data):
        """Sends a byte array to server through UDP"""
        self.__write_udp(control_id, 'r', data)


def setup(device_id, server_ip, server_port):
    """Set up server connection"""
    return UniotyHub(device_id, server_ip, server_port)

def main():
    """The main function. For testing only"""
    hub = setup(0x01, 'localhost', 25555)
    # UDP test writes
    hub.write_udp_float(0x01, 9.45)
    hub.write_udp_int(0x02, 1999)
    hub.write_udp_byte(0x03, 0xff)
    hub.write_udp_string(0x04, "hello!")
    hub.write_udp_raw(0x05, bytearray([0x01, 0x03, 0x05]))
    # TCP test writes
    hub.connect_tcp()
    hub.write_tcp_float(0x06, 0.02)
    hub.write_tcp_int(0x07, 2006)
    hub.write_tcp_byte(0x08, 0xff)
    hub.write_tcp_string(0x09, "world!")
    hub.write_tcp_raw(0x0a, bytearray([0x00, 0x02, 0x04]))


if __name__ == "__main__":
    main()
