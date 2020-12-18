import socket, time

TCP_IP = '192.168.178.182'
TCP_PORT = 5333

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((TCP_IP, TCP_PORT))
time.sleep(1)
s.sendall(b'Start\r\n')
s.close()