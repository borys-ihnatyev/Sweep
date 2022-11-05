//
//  Payload.swift
//  MyMenuBar
//
//  Created by Borys Ihnatyev on 24.09.2022.
//

import Foundation

enum EventName: String {
    case confirmEdit
    case watchToggle
}

struct TrackInfo: Decodable {
    let fullName: String
    let fullTitle: String
    let title: String
    
    let artistsString: String
    let remixArtistsString: String
    
    let isRemix: Bool
    let mixType: String
}

func event(_ eventName: EventName) -> String {
    return eventName.rawValue
}

struct ParsedPath: Decodable {
    let root: String
    let dir: String
    let base: String
    let ext: String
    let name: String
}

struct FileLocation: Decodable {
    let parsedPath: ParsedPath
    let path: String
}

struct ConfirmEditRequest: Decodable {
    let id: String
    let wasEdited: Bool
    let location: FileLocation
    let trackInfo: TrackInfo
}

struct ConfirmEditResponse: Encodable {
    let requestId: String
    let resolution: ConfirmEditResponseResolution
}

enum ConfirmEditResponseResolution: Encodable {
    enum CodingKeys: String, CodingKey {
        case type, newName
    }
    
    case allow
    case deny
    case rename(newName: String)
    
    func encode(to encoder: Encoder) throws {
        var container = encoder.container(keyedBy: CodingKeys.self)
        switch self {
        case .allow:
            try container.encode("allow", forKey: .type)
        case .deny:
            try container.encode("deny", forKey: .type)
        case .rename(let newName):
            try container.encode("rename", forKey: .type)
            try container.encode(newName, forKey: .newName)
            
        }
    }
}

struct WatchToggleRequest: Encodable {
    let enabled: Bool
}

