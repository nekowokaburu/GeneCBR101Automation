import sys

#print(sys.argv)

f = open("C:\\WorkRoot\\GeneRev4\\GeneRev4Py\\out.txt", "w")
for arg in sys.argv:
    f.write(str(arg))
f.close()