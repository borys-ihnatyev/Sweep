//
//  NotificationService.swift
//  MyMenuBar
//
//  Created by Borys Ihnatyev on 24.09.2022.
//

import Foundation
import UserNotifications
import Combine

enum EditResponseOption: String {
    case allow, deny, rename
}


func editResponseActionId(_ response: EditResponseOption) -> String {
    return response.rawValue
}

func editResponseOption(fromActionIdentifier identifier: String) -> EditResponseOption {
    switch identifier {
    case UNNotificationDismissActionIdentifier: return .deny
    case UNNotificationDefaultActionIdentifier: return .rename
    default:
        return EditResponseOption(rawValue: identifier) ?? .deny
    }
}

protocol NotificationServiceDelegate {
    func userRespond(forRequestId id: String, with option: EditResponseOption)
}

class NotificationService: NSObject, UNUserNotificationCenterDelegate {
    private let notificationCenter = UNUserNotificationCenter.current()
    private let confirmEditCategory = UNNotificationCategory(
        identifier: event(.confirmEdit),
        actions: [
            UNNotificationAction(identifier: editResponseActionId(.allow), title: "Allow"),
            UNNotificationAction(identifier: editResponseActionId(.deny), title: "Deny"),
            UNNotificationAction(identifier: editResponseActionId(.rename), title: "Rename")
        ],
        intentIdentifiers: [],
        options: [.customDismissAction]
    )
    
    var delegate: NotificationServiceDelegate?
    
    override init() {
        super.init()
        notificationCenter.delegate = self
        notificationCenter.setNotificationCategories([confirmEditCategory])
    }
    
    func userNotificationCenter(
        _ center: UNUserNotificationCenter,
        didReceive response: UNNotificationResponse,
        withCompletionHandler completionHandler: @escaping () -> Void
    ) {
        let content = response.notification.request.content
        let categoryIdentifier = content.categoryIdentifier
        
        if categoryIdentifier == confirmEditCategory.identifier {
            delegate?.userRespond(
                forRequestId: content.userInfo["requestId"] as! String,
                with: editResponseOption(fromActionIdentifier: response.actionIdentifier)
            )
        }
        
        completionHandler()
    }
    
    func notification(for request: ConfirmEditRequest) async throws  {
        let authorized = try await requesNotificationAuth()
        
        guard authorized else {
            return;
        }
        
        let settings = await notificationCenter.notificationSettings()
        
        guard settings.authorizationStatus == .authorized else {
            return;
        }
        
        let id = UUID().uuidString
        
        let content = UNMutableNotificationContent()
        content.title = "Rename?"
        content.subtitle = request.location.parsedPath.name
        content.body = request.trackInfo.fullName
        content.userInfo = ["requestId": request.id]
        
        content.categoryIdentifier = confirmEditCategory.identifier
        
        let request = UNNotificationRequest(identifier: id, content: content, trigger: nil)
        
        try await UNUserNotificationCenter
            .current()
            .add(request)
        
    }
    
    private func requesNotificationAuth() async throws -> Bool  {
        return try await notificationCenter.requestAuthorization(
            options: [.alert, .badge, .sound]
        )
    }
}
