import ctypes
MessageBox = ctypes.windll.user32.MessageBoxA
MessageBox(None, 'Hello World', 'AutoMovieArchive', 0)