//
//  ContentView.swift
//  MacOS
//
//  Created by Borys Ihnatyev on 20.09.2022.
//

import SwiftUI
import UserNotifications

struct ContentView: View {
    var body: some View {
        VStack {
            Button {
                NSApplication.shared.terminate(nil)
            } label: {
                Text("Quit")
            }
        }
        .padding()
        .frame(width: 150)
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
    }
}
