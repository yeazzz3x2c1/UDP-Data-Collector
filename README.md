# UDP-Data-Collector
This project contains two parts: a server and a client. This system allows multiple clients to upload files to the server simultaneously using the UDP protocol, while ensuring the files' integrity. The server is developed using C#, and the client is developed using Python.

We provide a simple example in the "Data Collector API" folder; check the file "client_codes_example.py" for more details. This system could make it very easy for you to transmit files from the client to the server.

## A Simple Client Example:
In this example code, the client will transmit a file called "Example.png" to the server, and you can find the file in the server folder at "./Receive/Received.png". Hope you enjoy!
```python
from Data_Collector_API import Data_Updater

updater = Data_Updater("127.0.0.1", 35525)
updater.Upload_File(File_Path="Example.png", Target_Path="Received.png") 
```
