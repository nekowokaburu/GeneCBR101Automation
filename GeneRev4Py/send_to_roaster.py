# Python client impl.
import socket, time, sys, re
debug = False

def main():
    TCP_IP = '192.168.178.182'
    TCP_PORT = 5333
    if debug:
        f = open("C:\\WorkRoot\\GeneRev4\\GeneRev4Py\\out.txt", "a") #Debug out file

    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.connect((TCP_IP, TCP_PORT))
    arg = str(sys.argv[1])
    arg += ';'

    if not '\r\n' in arg:
        arg = arg + '\r\n'
    send = arg
    received = 'x'
    if 'temp' in send:
        while re.findall("\d+", send) != re.findall("\d+", received):
            s.sendall(send.encode())
            #start = "Start\r\n"
            #s.send(start.encode())

            ### Debug PID temp from Artisan, not so well but ok working
            if debug:
                f.write('sent ' + arg)

            # Look for the response    
            received = ''
            while True:
                received += s.recv(48).decode()
                if '\n' in received:
                    break
            # print('Received: ' + received + ' send: ' + send + 'R in S: ' )
            # print(received in send)
            # print(send in received)
            # print(send.find(received))
            # print(re.findall("\d+", send))
            # print(re.findall("\d+", received))
            # print(re.findall("\d+", send) != re.findall("\d+", received))

        if debug:
            f.write('received ' + str(received))

    else:
        s.sendall(send.encode())

    s.close()
    if debug:
        f.close()





if __name__ == "__main__":
    main()