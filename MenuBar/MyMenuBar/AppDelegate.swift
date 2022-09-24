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
class AppDelegate: NSObject, NSApplicationDelegate {
    private var statusItem: NSStatusItem!
    private var popover: NSPopover!
    private var messagingService: MessagingService!
    
    func applicationDidFinishLaunching(_ notification: Notification) {
        statusItem = NSStatusBar.system.statusItem(withLength: NSStatusItem.variableLength);
        
        if let button = statusItem.button {
            button.image = NSImage(systemSymbolName: "eye", accessibilityDescription: "Sweep app")
            button.action = #selector(togglePopover)
        }
        
        popover = NSPopover()
        popover.contentSize = NSSize(width: 150, height: 300)
        popover.behavior = .transient
        popover.contentViewController = NSHostingController(rootView: ContentView())
        messagingService = MessagingService()
    }
    
    
    @objc func togglePopover() {
        popover.isShown ? hidePopover() : showPopover()
    }
    
    
    func showPopover() {
        if let button = statusItem?.button {
            popover.show(relativeTo: button.bounds, of: button, preferredEdge: .maxY)
        }
    }
    
    func hidePopover() {
        popover.performClose(nil)
    }
}
