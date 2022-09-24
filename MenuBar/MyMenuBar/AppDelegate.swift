//
//  AppDelegate.swift
//  MyMenuBar
//
//  Created by Leonardo Atalla on 7/22/21.
//
import Cocoa
import SwiftUI
import SocketIO

@main
class AppDelegate: NSObject, NSApplicationDelegate, MessagingServiceDelegate, NotificationServiceDelegate {
    private var statusItem: NSStatusItem!
    private var popover: NSPopover!
    private var messagingService: MessagingService!
    private var notificationService: NotificationService!
    
    func applicationDidFinishLaunching(_ notification: Notification) {
        setupMenuBar()

        notificationService = NotificationService()
        notificationService.delegate = self
        messagingService = MessagingService()
        messagingService.deleagate = self
    }
    
    private func setupMenuBar() {
        popover = NSPopover()
        popover.contentSize = NSSize(width: 150, height: 300)
        popover.behavior = .transient
        popover.contentViewController = NSHostingController(rootView: ContentView())
        
        statusItem = NSStatusBar.system.statusItem(withLength: NSStatusItem.variableLength);
        let button = statusItem.button!
        button.image = NSImage(systemSymbolName: "eye", accessibilityDescription: "Sweep app")
        button.action = #selector(togglePopover)
    }
    
    
    @objc func togglePopover() {
        popover.isShown ? hidePopover() : showPopover()
    }
    
    
    func showPopover() {
        let button = statusItem.button!
        popover.show(relativeTo: button.bounds, of: button, preferredEdge: .maxY)
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
        do {
            switch option {
            case .allow:
                try messagingService.confirmEdit(.init(requestId: id, resolution: .allow))
            case .deny:
                try messagingService.confirmEdit(.init(requestId: id, resolution: .deny))
            default:
                print("Not supported option")
            }
        } catch {
            print("Send error")
        }
    }
}
