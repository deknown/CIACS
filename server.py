import socket
import threading
import time
import sys
from datetime import datetime
from queue import Queue
import struct
import signal
import json

NUMBER_OF_THREADS = 2
JOB_NUMBER = [1, 2]
queue = Queue()
text_file = 'fileProj.json'
receive = 'client-info.json'
structure_file = 'server-info.json'
clients_list = 'clients.json'

COMMANDS = {'help':['Shows this help'],
            'list':['Lists connected clients'],
            'get':['Gets file from client. Usage: "address CIACS get"'],
            'select':['Selects a client by its index. Takes index as a parameter'],
            'quit':['Stops current connection with a client. To be used when client is selected'],
            'shutdown':['Shuts server down'],
           }

CLIENT_COMMANDS = ["b'GET locations'", "b'POST request'", "b'POST location'"]


class MultiServer(object):

    def __init__(self):
        self.host = '192.168.10.2'
        self.port = 65100
        self.socket = None
        self.all_connections = []
        self.all_addresses = []

    def print_help(self):
        for cmd, v in COMMANDS.items():
            print("{0}:\t{1}".format(cmd, v[0]))
        return

    def register_signal_handler(self):
        signal.signal(signal.SIGINT, self.quit_gracefully)
        signal.signal(signal.SIGTERM, self.quit_gracefully)
        return

    def quit_gracefully(self, signal=None, frame=None):
        print('\nQuitting gracefully')
        for conn in self.all_connections:
            try:
                conn.shutdown(2)
                conn.close()
            except Exception as e:
                print('Could not close connection %s' % str(e))
                # continue
        self.socket.close()
        sys.exit(0)

    def socket_create(self):
        try:
            self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM, socket.IPPROTO_TCP)
            print("Socket created")
        except socket.error as msg:
            print("Socket creation error: " + str(msg))
            sys.exit(1)
        self.socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        return

    def socket_bind(self):
        """ Bind socket to port and wait for connection from client """
        try:
            self.socket.bind((self.host, self.port))
            self.socket.listen(1)
        except socket.error as e:
            print("Socket binding error: " + str(e))
            time.sleep(5)
            self.socket_bind()
        return

    def accept_connections(self):
        """ Accept connections from multiple clients and save to list """
        for c in self.all_connections:
            c.close()
        self.all_connections = []
        self.all_addresses = []
        while 1:
            try:
                conn, address = self.socket.accept()
                conn.setblocking(1)
                client_hostname = socket.gethostname()
                address = address + (client_hostname,)
            except Exception as e:
                print('Error accepting connections: %s' % str(e))
                # Loop indefinitely
                continue
            self.all_connections.append(conn)
            self.all_addresses.append(address)
            print('\nConnection has been established: {0} ({1})'.format(address[-1], address[0]) +' '+ str(datetime.now()))
            data = conn.recv(85000)
            if not data:
                break
            elif str(data) in CLIENT_COMMANDS[0]:
                self.send_file(conn)
                continue
            elif str(data) in CLIENT_COMMANDS[1]:
                continue
            elif str(data) in CLIENT_COMMANDS[2] or data.startswith(b'POST location'):
                self.receive_file(conn, data)
                continue
            elif data:
                clients_list1 = open(clients_list)
                add_to_list = json.load(clients_list1)
                add_to_list['ip'] = address[0]
                with open(clients_list, "w") as clw:
                    clw.write(str(json.dumps(add_to_list)))
                    clw.close()
                self.get_file(conn, data)
                import jsonreader
                jsonreader.read()
                continue
        return

    def start_turtle(self):
        """ Interactive prompt for sending commands remotely """
        while True:
            cmd = input('turtle> ')
            if cmd == 'list':
                self.list_connections()
                continue
            elif 'select' in cmd:
                target, conn = self.get_target(cmd)
                if conn is not None:
                    self.send_target_commands(target, conn)
            elif cmd == 'shutdown':
                    queue.task_done()
                    queue.task_done()
                    print('Server shutdown')
                    break
                    # self.quit_gracefully()
            elif cmd == 'help':
                self.print_help()
            elif cmd == '':
                pass
            else:
                print('Command not recognized')
        return

    def list_connections(self):
        """ List all connections """
        results = ''
        for i, conn in enumerate(self.all_connections):
            try:
                conn.send(str.encode(' '))
                conn.recv(20480)
            except:
                del self.all_connections[i]
                del self.all_addresses[i]
                continue
            results += str(i) + '   ' + str(self.all_addresses[i][0]) + '   ' + str(
                self.all_addresses[i][1]) + '   ' + str(self.all_addresses[i][2]) + '\n'
        print('----- Clients -----' + '\n' + results)
        return

    def get_target(self, cmd):
        """ Select target client
        :param cmd:
        """
        target = cmd.split(' ')[1]
        try:
            target = int(target)
        except:
            print('Client index should be an integer')
            return None, None
        try:
            conn = self.all_connections[target]
        except IndexError:
            print('Not a valid selection')
            return None, None
        if 'get' in cmd:
            print("You have sent command to " + str(self.all_addresses[target][2]))
        else:
            print("You are now connected to " + str(self.all_addresses[target][2]))
        return target, conn

    def read_command_output(self, conn):
        """ Read message length and unpack it into an integer
        :param conn:
        """
        raw_msglen = self.recvall(conn, 4)
        if not raw_msglen:
            return None
        msglen = struct.unpack('>I', raw_msglen)[0]
        # Read the message data
        return self.recvall(conn, msglen)

    def recvall(self, conn, n):
        """ Helper function to recv n bytes or return None if EOF is hit
        :param n:
        :param conn:
        """
        data = b''
        while len(data) < n:
            packet = conn.recv(n - len(data))
            if not packet:
                return None
            data += packet
        return data

    def send_target_commands(self, target, conn):
        """ Connect with remote target client
        :param conn:
        :param target:
        """
        conn.send(str.encode(" "))
        cwd_bytes = self.read_command_output(conn)
        cwd = str(cwd_bytes, "utf-8")
        print(cwd, end="")
        while True:
            try:
                cmd = input()
                if len(str.encode(cmd)) > 0:
                    conn.send(str.encode(cmd))
                    cmd_output = self.read_command_output(conn)
                    client_response = str(cmd_output, "utf-8")
                    print(client_response, end="")
                if cmd == 'quit':
                    break
            except Exception as e:
                print("Connection was lost %s" %str(e))
                break
        del self.all_connections[target]
        del self.all_addresses[target]
        return

    def get_file(self, conn, data):
        with open(text_file, "wb") as fw:
            print("Receiving data of client PC configuration")
            while True:
                print('receiving')
                if data == b'BEGIN':
                    continue
                elif data == b'ENDED':
                    print('Breaking from file write')
                    break
                else:
                    print('Received.')
                    fw.write(data)
                    print('Wrote to file.')
                    break
            fw.close()
            conn.close()
            return

    def send_file(self, conn):
        with open(structure_file, 'rb+') as fa:
            print("Sending structure file.")
            while True:
                data = fa.read(4096)
                conn.send(data)
                if not data:
                    break
            fa.close()
            conn.close()
            print("Sent file.")
        return

    def receive_file(self, conn, data):
        with open(receive, "wb") as fw:
            print("Receiving updated data")
            while True:
                print('receiving')
                print(type(data))
                if data == b'BEGIN':
                    continue
                elif data == b'ENDED':
                    print('Breaking from file write')
                    break
                else:
                    data.replace(b'POST location', b'')
                    print('Received: ', data.decode('utf-8'))
                    fw.write(data)
                    print('Wrote to file', data.decode('utf-8'))
                    break
            fw.close()
            conn.close()
            print("Received..")
            return

def create_workers():
    """ Create worker threads (will die when main exits) """
    server = MultiServer()
    server.register_signal_handler()
    for _ in range(NUMBER_OF_THREADS):
        t = threading.Thread(target=work, args=(server,))
        t.daemon = True
        t.start()
    return


def work(server):
    """ Do the next job in the queue (thread for handling connections, another for sending commands)
    :param server:
    """
    while True:
        x = queue.get()
        if x == 1:
            server.socket_create()
            server.socket_bind()
            server.accept_connections()
        if x == 2:
            server.start_turtle()
        queue.task_done()
    return


def create_jobs():
    """ Each list item is a new job """
    for x in JOB_NUMBER:
        queue.put(x)
    queue.join()
    return


def main():
    create_workers()
    create_jobs()


if __name__ == '__main__':
    main()