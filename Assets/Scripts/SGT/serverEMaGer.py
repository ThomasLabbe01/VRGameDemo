# -*- coding: utf-8 -*-
"""
Created on Tue Nov  7 12:01:04 2023

@author: THLAB40
"""
import socket
from SGT import SGT
import PrepareClassifierEMaGer
import libemg
import pickle
from libemg.utils import make_regex
import numpy as np
import time
import threading
import libemg_subclass


HOST = 'localhost'  # Host IP address
PORT = 12346       # Port number to listen on
exit_event = threading.Event()
sgt_list = [None]  # Using a list to store the SGT object

def TaskSGT(SGTFlag, client_socket, sgt):
    while SGTFlag:
        # Receive data from Unity
            dataSGT = client_socket.recv(1024).decode('utf-8')
            # print(f"Received data: {dataSGT}")
            if dataSGT:
                SGTFlag, sgt = ProcessReceivedData(client_socket, dataSGT, sgt, SGTFlag)
                sgt_list[0] = sgt  # Update the value in the list
    print("SGT training over")
    exit_event.set()  # Set the event when thread_a finishes

def TaskLiveDisplay(SGTFlag, odh, client_socket):
    # Set the desired batch size
    batch_size = 40
    data_buffer = np.zeros((0, 64))  # Assuming 64 is the number of features in your data

    while SGTFlag and not exit_event.is_set():

        dataLiveDisplay = odh.get_data()
        # Take only the last 25 samples for processing
        data_buffer = dataLiveDisplay[-batch_size:]

        # Process and send the data
        mean_data = np.mean(np.absolute(data_buffer), axis=0)
        new_min = 10
        new_max = 1200
        old_min = np.min(mean_data)
        old_max = np.max(mean_data)
        scaled_array = (mean_data - old_min) * (new_max - new_min) / (old_max - old_min) + new_min
        data_to_send = np.array2string(scaled_array.flatten().astype(int), formatter={'int': lambda x: f"{x:04d}"}, separator=';')[1:-1]
        if data_to_send:
            # Send data to TCP server
            client_socket.send(data_to_send.encode('utf-8'))

        # Introduce a delay before the next iteration
        time.sleep(0.02)
    print("LiveDisplay Over")        

def StartServer():
    SGTFlag = True
    
    try:
        # Create a TCP/IP socket
        server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        # Bind the socket to a specific address and port
        server_socket.bind((HOST, PORT))
        # Listen for incoming connections
        server_socket.listen(1)
        print(f"Server started on {HOST}:{PORT}")

        # Accept a client connection
        client_socket, client_address = server_socket.accept()
        print(f"Connection established with {client_address[0]}:{client_address[1]}")

        thread_a = threading.Thread(target=lambda: TaskSGT(SGTFlag, client_socket, sgt_list[0]))
        thread_b = threading.Thread(target=lambda: TaskLiveDisplay(SGTFlag, odh, client_socket))

        try:
        # Start both threads
            thread_a.start()
            #thread_b.start()

            thread_a.join()

            exit_event.set()

            #thread_b.join()
        except Exception as e:
            print(f"Error processing data: {e}")
        p.kill()
        odh.stop_listening()
        sgt = sgt_list[0]
        oc = PrepareClassifierEMaGer.prepare_classifier(sgt.num_reps, sgt.input_count, sgt.output_folder)
        libemg_subclass.start_live_classifier(oc)

        
    except Exception as e:
        print(f"Server error: {e}")      
    finally:
        client_socket.close()
        server_socket.close()

          

def ProcessReceivedData(client_socket, data, sgt=None, SGTFlag=True):
    print("Received data from Unity:", data)
    if (data[0] == 'I'): #Initialization
        #SGT(data_handler, num_reps, time_per_reps, time_bet_rep, inputs_names, output_folder)
        split_idx = data.find(' ')
        sgt = SGT(odh, int(data[1]), int(data[2]), int(data[3]), data[4 : split_idx ], data[split_idx + 1:])
        return SGTFlag, sgt
    else:
        SGTFlag = sgt._collect_data(data[0])
        return SGTFlag, sgt
    

def SendData(client_socket, data):
    # Send response back to Unity client
    print(f"We made it")
    client_socket.send(data.encode('utf-8'))


if (__name__ == "__main__"):
    p = libemg.streamers.emager_streamer() #process to start myo giving out data
    odh = libemg.data_handler.OnlineDataHandler(max_buffer=25) #online data handler: process to start grabbing EMaGer data
    odh.start_listening()
    StartServer()