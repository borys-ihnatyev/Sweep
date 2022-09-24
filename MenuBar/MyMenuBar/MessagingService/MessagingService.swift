//
//  MessagingService.swift
//  MyMenuBar
//
//  Created by Borys Ihnatyev on 24.09.2022.
//

import Foundation
import SocketIO

class MessagingService {
    private let socketManager = SocketManager(socketURL: URL(string: "ws://localhost:3737")!, config: [.log(true), .compress])
    
    init() {
        let socket = socketManager.defaultSocket
        
        socket.on(clientEvent: .connect) {data, ack in
            print("Socket connected")
        }
        
        socket.on(event(.confirmEdit)) {data, ack in
            do {
                let request: ConfirmEditRequest = try self.decodeMessage(data)
                print(event(.confirmEdit), request);
                socket.emit(event(.confirmEdit), try self.encodeMessage(ConfirmEditResponse.allow))
                socket.emit(event(.confirmEdit), try self.encodeMessage(ConfirmEditResponse.allow))
                socket.emit(event(.confirmEdit), try self.encodeMessage(ConfirmEditResponse.rename(newName: "hello - world")))
            } catch {
                print("decode error", error)
            }
        }
        
        socket.connect()
    }
    
    func decodeMessage<T>(_ data: [Any]) throws -> T where T : Decodable {
        let payload = data[0]
        let jsonData = try JSONSerialization.data(withJSONObject: payload)
        return try JSONDecoder().decode(T.self, from: jsonData)
    }
    
    func encodeMessage<T>(_ message: T) throws -> [String: Any] where T: Encodable  {
        let data = try JSONEncoder().encode(message)
        return try JSONSerialization.jsonObject(with: data, options: .allowFragments) as! [String: Any]
    }
}
