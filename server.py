import socket

host = '127.0.0.1'
port = 80700

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as stream:
    stream.connect((host, port))
    stream.sendall(b'Hello world')
    data = stream.recv(1024)
print('Received: ', repr(data))