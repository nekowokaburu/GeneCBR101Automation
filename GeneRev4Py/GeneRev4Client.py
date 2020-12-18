import socket

TCP_IP = '192.168.178.182'
TCP_PORT = 5333
BUFFER_SIZE = 1024
#MESSAGE = "Hello, World!"

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((TCP_IP, TCP_PORT))
#s.send(MESSAGE)

buffer = ''
continue_recv = True

# while continue_recv:
try:
    buffer += s.makefile().readline()
    if "\r\n" in str(buffer):
        continue_recv = False
except:
    continue_recv = False

s.close()
 
print(float(buffer))