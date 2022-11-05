//
//  AppDelegate.swift
//  MyMenuBar
//
//  Created by Leonardo Atalla on 7/22/21.
//
import SwiftUI
import SocketIO
import Combine

class AppDelegate: NSObject, NSApplicationDelegate, MessagingServiceDelegate, NotificationServiceDelegate {
    let messagingService = MessagingService()
    let notificationService = NotificationService()
    
    override init() {
        super.init()
        notificationService.delegate = self
        messagingService.deleagate = self
    }
    
    func request(_ payload: ConfirmEditRequest) {
        Task {
            print("REQUEST", payload);
            try await notificationService.notification(for: payload)
        }
    }
    
    func userRespond(forRequestId id: String, with option: EditResponseOption) {
        print("respond with", id, option)
        switch option {
        case .allow:
            messagingService.confirmEdit(.init(requestId: id, resolution: .allow))
        case .deny:
            messagingService.confirmEdit(.init(requestId: id, resolution: .deny))
        default:
            print("Not supported option")
        }
    }
}
