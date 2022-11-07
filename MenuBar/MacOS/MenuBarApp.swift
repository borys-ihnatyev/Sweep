//
//  ContentView.swift
//  MacOS
//
//  Created by Borys Ihnatyev on 20.09.2022.
//

import SwiftUI
import UserNotifications

@main
struct MenuBarApp: App {
    @NSApplicationDelegateAdaptor var appDelegate: AppDelegate
    @StateObject var settings = AppSettingsStore()
    
    var icon: String {
        if let settings = settings.value {
            return settings.watchEnabled ? "eye" : "eye.slash"
        }

        return "eye"
    }
    
    var body: some Scene {
        MenuBarExtra(
            "MenuBar",
            systemImage: icon
        ) {
            
        }
        .onChange(of: settings.watchEnabled, perform: appDelegate.messagingService.useWatcher)
        .onChange(of: settings.notificationsEnabled, perform: appDelegate.messagingService.useNotifications)
    }
}
