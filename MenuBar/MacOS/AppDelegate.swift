//
//  AppDelegate.swift
//  MyMenuBar
//
//  Created by Leonardo Atalla on 7/22/21.
//
import Cocoa
import SwiftUI
import SocketIO
import Combine

@main
class AppDelegate: NSObject, NSApplicationDelegate, MessagingServiceDelegate, NotificationServiceDelegate {
    private var statusItem: NSStatusItem!
    private var popover: NSPopover!
    private var messagingService = MessagingService()
    private let notificationService = NotificationService()
    
    private let appSettings = AppSettings()
    private var unsubscribeAppSettingsWatchEnabled: AnyCancellable?
    
    override init() {
        super.init()
        notificationService.delegate = self
        messagingService.deleagate = self
    }
    
    func applicationDidFinishLaunching(_ notification: Notification) {
        setupMenuBar()
    }
    
    private func setupMenuBar() {
        popover = NSPopover()
        popover.contentSize = NSSize(width: 150, height: 300)
        popover.setValue(true, forKeyPath: "shouldHideAnchor")
        popover.behavior = .transient
        popover.contentViewController = NSHostingController(
            rootView: MenuBarView()
                .environmentObject(appSettings)
        )
        
        statusItem = NSStatusBar.system.statusItem(withLength: NSStatusItem.variableLength);
        let button = statusItem.button!
        button.image = menuBarIconEnabled
        button.action = #selector(togglePopover)
        
        unsubscribeAppSettingsWatchEnabled = appSettings.$watchEnabled.sink { enabled in
            button.image = enabled ? menuBarIconEnabled : menuBarIconDisabled
            self.messagingService.toggleWatch(WatchToggleRequest(enabled: enabled))
        }
    }
    
    
    @objc func togglePopover() {
        popover.isShown ? hidePopover() : showPopover()
    }
    
    
    func showPopover() {
        let button = statusItem.button!
        popover.show(relativeTo: button.bounds, of: button, preferredEdge: .maxY)
        NSApplication.shared.activate(ignoringOtherApps: true)
    }
    
    func hidePopover() {
        popover.performClose(nil)
    }
    
    func request(_ payload: ConfirmEditRequest) {
        Task {
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
