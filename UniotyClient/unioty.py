"""A library for communication with a Unioty server from a Unioty device"""
import socket
import struct


class UniotyPusher(object):
    """UniotyPusher writes control data from a device to a host"""

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

    def connect_tcp(self):
        """Sets up TCP connection to the server for writing"""
        if self.sock_tcp != None:
            self.sock_tcp.close()
        self.sock_tcp = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        intro_message = bytearray()
        intro_message.append(0x01) # opcode for PUSH
        intro_message.append(self.device_id)
        self.sock_tcp.connect(self.server_address)
        self.sock_tcp.sendall(intro_message)

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


class UniotyPoller(object):
    """UniotyPoller reads host's controls through the network"""

    def __init__(self, server_ip, server_port):
        self.server_address = (server_ip, server_port)

    def read_host_control(self, control_id, callback):
        """Connects to host and invokes callback whenever control data is changed"""
        sock_tcp = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock_tcp.connect(self.server_address)
        intro_message = bytearray()
        intro_message.append(0x00) # opcode for POLL
        intro_message.append(control_id) # control id as byte
        sock_tcp.sendall(intro_message)
        while True:
            recv_ctrl_id = sock_tcp.recv(1)
            # Assert it is the same control we subscribed
            #assert recv_ctrl_id == control_id
            data_type = sock_tcp.recv(1)
            data_len = sock_tcp.recv(1)
            data_raw = sock_tcp.recv(int(data_len.encode('hex'), 16))
            data = None
            # Convert data
            if data_type == 's' or data_type == 'r':
                data = data_raw
            else:
                data = struct.unpack('<'+data_type, data_raw)
            # Invoke callback
            callback(data)


def setup_pusher(device_id, server_ip, server_port):
    """Sets up server connection for pushing device control"""
    return UniotyPusher(device_id, server_ip, server_port)

def setup_poller(server_ip, server_port):
    """Sets up for reading host control"""
    return UniotyPoller(server_ip, server_port)

def main():
    """Main function for testing"""
    poller = setup_poller('localhost', 25556)
    poller.read_host_control(0x00, print_to_screen)

def print_to_screen(data):
    """Print data to screen"""
    print data

if __name__ == "__main__":
    main()
