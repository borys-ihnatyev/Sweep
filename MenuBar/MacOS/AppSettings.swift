//
//  AppSettings.swift
//  MyMenuBar
//
//  Created by Borys Ihnatyev on 25.09.2022.
//

import Foundation

class AppSettings: ObservableObject {
    @Published var watchEnabled: Bool
    @Published var autoRenameEnabled: Bool
    
    init(watchEnabled: Bool, autoRenameEnabled: Bool) {
        self.watchEnabled = watchEnabled
        self.autoRenameEnabled = autoRenameEnabled
    }
}

class AppSettingsStore: ObservableObject {
    @Published var value: Optional<AppSettings> = .none;
}
