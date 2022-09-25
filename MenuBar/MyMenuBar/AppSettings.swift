//
//  AppSettings.swift
//  MyMenuBar
//
//  Created by Borys Ihnatyev on 25.09.2022.
//

import Foundation

class AppSettings: ObservableObject {
    @Published var watchEnabled = UserDefaults.standard.bool(forKey: "watchEnabled") {
        didSet {
            UserDefaults.standard.setValue(watchEnabled, forKey: "watchEnabled")
        }
    }
}
