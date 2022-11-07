//
//  MenuContent.swift
//  MenuBar
//
//  Created by Borys Ihnatyev on 06.11.2022.
//

import SwiftUI

struct MenuContent: View {
    var settings: AppSettings
    
    var body: some View {
        VStack {
            Button {
                settings.watchEnabled.toggle()
            } label: {
                Text(settings.watchEnabled ? "Disable" : "Enable")
            }.keyboardShortcut("e")
            
            Button {
                settings.notificationsEnabled.toggle()
            } label: {
                Text(settings.notificationsEnabled ? "Off notifications" : "On notifications")
            }.keyboardShortcut("a")
            
            Divider()
            
            Button {
                NSApplication.shared.terminate(nil)
            } label: {
                Text("Quit")
            }
            .keyboardShortcut("q")
        }
    }
}

struct MenuContent_Previews: PreviewProvider {
    static var previews: some View {
        MenuContent()
    }
}
