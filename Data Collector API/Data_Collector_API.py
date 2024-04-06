from datetime import datetime
import requests
import base64
import os
import socket
import time as time
import shutil
import traceback



class Data_Updater:
    def __init__(self, IP, Port):
        self.IP = IP
        self.Port = Port
        self.Data_Index = 0
        self.Maximum_Data_Length = 0
        self.Timeout = 5000
        self._Initialize()

    def _Init_Packet(self, Cmd, Data):
        self.Data_Index = self.Data_Index + 1
        self.Data_Index &= 0xff
        data_list = [self.Data_Index, Cmd]
        i = 0
        for d in Data:
            data_list.append(d)
        CRC = 0
        for d in data_list:
            CRC += d
        CRC &= 0xff
        data_list.append(CRC)
        return bytearray(data_list)


    def _Send_Data_And_Get_Response(self, Packet):
        UDP_IP = self.IP
        UDP_PORT = self.Port
        BUFFER_SIZE = 4096
        while(True):
            try:
                sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)  # UDP
                sock.settimeout(self.Timeout / 1000)
                sock.sendto(Packet, (UDP_IP, UDP_PORT))
                result, _ = sock.recvfrom(BUFFER_SIZE)
                if(result[0] != self.Data_Index):
                    print('Index Invaild, Except: ' + str(self.Data_Index) +
                          ', Got: ' + str(result[0]))
                    time.sleep(1)
                    continue
                if(Packet[1] != result[1]):
                    if(result[1] == 0xFF):
                        print('Server sent reset cmd.')
                        self._Initialize()
                        self.Data_Index = self.Data_Index - 1
                        self._Send_Data_And_Get_Response(
                            self._Get_File_Path_Packet(None, None, None, None, True))
                        self.Data_Index = self.Data_Index - 1
                    else:
                        print('CMD Invaild, Except: ' +
                              str(Packet[1]) + ', Got: ' + str(result[1]))

                    time.sleep(1)
                    continue
                i = 0
                CRC = 0
                for j in range(len(result) - 1):
                    CRC += result[j]
                CRC &= 0xff
                if(CRC != result[len(result) - 1]):
                    print('CRC Invaild, Except: ' + str(CRC) +
                          ', Got: ' + str(result[len(result) - 1]))
                    time.sleep(1)
                    continue
                break
            except:
                print(f'{round(time.time() * 1000)} Socket Exception occur:')
                print(traceback.format_exc())
                continue
        return result


    def _Decode_Data(Row_Data, Offset, Length):
        data = 0
        for i in range(Length):
            data <<= 8
            data |= Row_Data[Offset + Length - i + 1]
        return data


    def _Encode_Data(data, len):
        final_data = []
        for j in range(len):
            final_data.append(data & 0xff)
            data >>= 8
        return bytearray(final_data)


    def _Get_File_Path_Packet(self, milli, file_size, path_len, path, cache):
        if(not cache):
            self.milli_bytes = milli
            self.file_size_bytes = file_size
            self.path_len_bytes = path_len
            self.path_bytes = path
        return self._Init_Packet(1, self.milli_bytes + self.file_size_bytes + self.path_len_bytes + self.path_bytes)


    def _Upload_File(self, Timestamp, Path, Data):
        path = Path.encode('utf8')
        path_len = len(path)
        total_len = len(Data)
        milli = Data_Updater._Encode_Data(Timestamp, 8)
        # 1 = Register File Path
        self._Send_Data_And_Get_Response(self._Get_File_Path_Packet(
            milli, Data_Updater._Encode_Data(total_len, 4), Data_Updater._Encode_Data(path_len, 2), path, False))
        # 2 = Upload File
        # [CMD, Timestamp(File_Index;8Bytes), Data_Length(2Bytes), Data]
        current_index = 0
        while(True):
            upload_len = total_len - current_index
            if(upload_len > self.Maximum_Data_Length):
                upload_len = self.Maximum_Data_Length
            #print(f'Sending... {current_index} ~ {current_index + upload_len}')
            result = self._Send_Data_And_Get_Response(self._Init_Packet(
                2, milli + Data_Updater._Encode_Data(upload_len, 4) + Data[current_index: current_index + upload_len]))
            server_current_img_len = Data_Updater._Decode_Data(result, 8, 4)
            if(server_current_img_len != current_index + upload_len):
                if(server_current_img_len < current_index):
                    current_index = 0
                rollbck_len = server_current_img_len - current_index
                # Rollback
                print(
                    f'Length Check Error, Except: {current_index + upload_len}, Got: {server_current_img_len}, Rolling back...')
                self._Send_Data_And_Get_Response(self._Init_Packet(3, milli + Data_Updater._Encode_Data(rollbck_len, 4)))
                continue
            current_index += upload_len
            if(current_index == total_len):
                break


    def _Initialize(self):
        print('Requeting server data setting...')
        data = self._Send_Data_And_Get_Response(self._Init_Packet(0, []))
        self.Maximum_Data_Length = Data_Updater._Decode_Data(data, 0, 4)
        self.Timeout = Data_Updater._Decode_Data(data, 4, 2)
        print(f'File Maxmimum Slice: {self.Maximum_Data_Length}, Timeout: {self.Timeout}')

    def _Get_File_Bytes(File_Path):
        file_size = -1
        with open(File_Path, 'rb') as f:
            data = f.read()
            while True:
                if(len(data) == file_size):
                    break
                file_size = len(data)
                time.sleep(1)
        return data

    def Upload_File(self, File_Path, Target_Path):
        try:
            file_data = Data_Updater._Get_File_Bytes(File_Path)
            Timestamp = round(os.path.getmtime(File_Path) * 1000)
            print(f'Uploading file... {File_Path}')
            print(f'Total Length: {len(file_data)}')
            self._Upload_File(Timestamp, f"./Receive/{Target_Path}", file_data)
        except:
            print(f'{round(time.time() * 1000)} File Uploading Failed:')
            print(traceback.format_exc())