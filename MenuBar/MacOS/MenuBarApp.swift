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
    @StateObject var settings = AppSettings()
    
    var body: some Scene {
        MenuBarExtra(
            "MenuBar",
            systemImage: settings.watchEnabled ? "eye" : "eye.slash"
        ) {
            VStack {
                Button {
                    settings.watchEnabled.toggle()
                } label: {
                    Text(settings.watchEnabled ? "Disable" : "Enable")
                }.keyboardShortcut("e")
                
                Divider()
                
                Button {
                    NSApplication.shared.terminate(nil)
                } label: {
                    Text("Quit")
                }
                .keyboardShortcut("q")
            }
        }.onChange(of: settings.watchEnabled, perform: appDelegate.messagingService.toggleWatch)
    }
}
