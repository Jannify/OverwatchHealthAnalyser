from mote import Mote
import sys
motes = Mote()
motes.configure_channel(1, 16, False)

r = int(sys.argv[1])
g = int(sys.argv[2])
b = int(sys.argv[3])

motes.clear()
motes.set_pixel(1, 0, r, g, b)
motes.set_pixel(1, 1, r, g, b)
motes.set_pixel(1, 2, r, g, b)      
motes.set_pixel(1, 3, r, g, b)
motes.set_pixel(1, 4, r, g, b)
motes.set_pixel(1, 5, r, g, b)
motes.set_pixel(1, 6, r, g, b)
motes.set_pixel(1, 7, r, g, b)
motes.set_pixel(1, 8, r, g, b)
motes.set_pixel(1, 9, r, g, b)
motes.set_pixel(1, 10, r, g, b)
motes.set_pixel(1, 11, r, g, b)
motes.set_pixel(1, 12, r, g, b)
motes.set_pixel(1, 13, r, g, b)
motes.set_pixel(1, 14, r, g, b)
motes.set_pixel(1, 15, r, g, b)
motes.show()