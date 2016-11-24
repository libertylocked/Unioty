"""A library for communication with a Unioty server from a Unioty device"""
import socket
import struct


class UniotyHub(object):
    """UniotyHub is a hub that communicates with a UniotyServer"""

    def __init__(self, device_id, server_ip, server_port):
        self.device_id = device_id
        self.server_address = (server_ip, server_port)
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    def write_analog_float(self, control_id, data):
        """Sends a float32 as analog data to server"""
        self.__write_analog(control_id, 'f', data)

    def write_analog_int(self, control_id, data):
        """Sends an int32 as analog data to server"""
        self.__write_analog(control_id, 'i', data)

    def write_analog_string(self, control_id, data):
        """Sends a string as analog data to server"""
        self.__write_analog(control_id, 's', data)

    def write_analog_byte(self, control_id, data):
        """Sends an unsigned byte as analog data to server"""
        self.__write_analog(control_id, 'B', data)


    def __write_analog(self, control_id, data_type, data):
        message = bytearray()
        message.append(self.device_id)
        # control_id is a byte
        message.append(control_id)
        # data_type is a char
        message.append(data_type)
        # data is packed into bytes
        if data_type == 's':
            message.extend(data)
        else:
            # Pack into little endian
            message.extend(struct.pack('<'+data_type, data))

        self.sock.sendto(message, self.server_address)


def setup(server_ip, server_port):
    """Set up server connection"""
    return UniotyHub(0x01, server_ip, server_port)

def main():
    """The main function. For testing only"""
    hub = setup('localhost', 25555)
    hub.write_analog_float(0x01, 9.45)
    hub.write_analog_int(0x02, 1999)
    hub.write_analog_byte(0x03, 0xff)
    hub.write_analog_string(0x04, "hello!")

if __name__ == "__main__":
    main()
