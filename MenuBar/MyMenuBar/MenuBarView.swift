//
//  ContentView.swift
//  MacOS
//
//  Created by Borys Ihnatyev on 20.09.2022.
//

import SwiftUI
import UserNotifications

struct MenuBarView: View {
    @EnvironmentObject var appSettings: AppSettings
    
    var body: some View {
        VStack {
            Button {
                appSettings.watchEnabled.toggle()
            } label: {
                Text(appSettings.watchEnabled ? "Disable" : "Enable")
                    .frame(maxWidth: .infinity, alignment: .leading)
            }.keyboardShortcut("e")
        
            Divider()
            
            Button {
                NSApplication.shared.terminate(nil)
            } label: {
                Text("Quit")
                    .frame(maxWidth: .infinity, alignment: .leading)
            }
            .keyboardShortcut("q")
        }
        .buttonStyle(.plain)
        .padding()
        .frame(width: 150)
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        MenuBarView()
    }
}
