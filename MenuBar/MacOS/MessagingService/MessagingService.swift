//
//  MessagingService.swift
//  MyMenuBar
//
//  Created by Borys Ihnatyev on 24.09.2022.
//

import Foundation
import SocketIO
import UserNotifications
import Combine

protocol MessagingServiceDelegate {
    func request(_ payload: ConfirmEditRequest)
}

class MessagingService {
    private let socketManager = SocketManager(
        socketURL: URL(string: "ws://localhost:3737")!,
        config: [.compress]
    )
    
    public var deleagate: MessagingServiceDelegate?
    
    init() {
        let socket = socketManager.defaultSocket
        
        socket.on(clientEvent: .connect) {data, ack in
            print("Socket connected")
        }
        
        socket.on(event(.confirmEdit)) {data, _ in
            self.onConfirmEdit(data)
        }
        
        socket.connect()
    }
    
    func confirmEdit(_ payload: ConfirmEditResponse) {
        emit(.confirmEdit, payload)
    }
    
    func toggleWatch(_ enabled: Bool) {
        emit(.watchToggle, WatchToggleRequest(enabled: enabled))
    }
    
    private func emit(_ name: EventName, _ payload: Encodable) {
        print("emit \(name), payload: \(payload)")
        do {
            let data = try encodeMessage(payload)
            socketManager.defaultSocket.emit(event(name), data)
        } catch {
            print("Failed to send \(name) with payload: \(payload), error: \(error.localizedDescription)")
        }
    }
    
    private func onConfirmEdit(_ data: [Any]) {
        do {
            let confirmEdit: ConfirmEditRequest = try self.decodeMessage(data)
            self.deleagate?.request(confirmEdit)
        } catch let error {
            print("confirmEdit eror:", error)
        }

    }
    
    private func decodeMessage<T>(_ data: [Any]) throws -> T where T : Decodable {
        let payload = data[0]
        let jsonData = try JSONSerialization.data(withJSONObject: payload)
        return try JSONDecoder().decode(T.self, from: jsonData)
    }
    
    private func encodeMessage<T>(_ message: T) throws -> [String: Any] where T: Encodable  {
        let data = try JSONEncoder().encode(message)
        return try JSONSerialization.jsonObject(with: data, options: .allowFragments) as! [String: Any]
    }
}
