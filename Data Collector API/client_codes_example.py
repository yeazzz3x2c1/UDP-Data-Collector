from Data_Collector_API import Data_Updater

updater = Data_Updater("127.0.0.1", 35525)

updater.Upload_File(File_Path="Example.png", Target_Path="Received.png") 