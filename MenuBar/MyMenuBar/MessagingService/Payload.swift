//
//  Payload.swift
//  MyMenuBar
//
//  Created by Borys Ihnatyev on 24.09.2022.
//

import Foundation

struct TrackInfo: Decodable {
    let fullName: String
    let fullTitle: String
    let title: String
    
    let artistsString: String
    let remixArtistsString: String
    
    let isRemix: Bool
    let mixType: String
}

enum EventName: String {
    case confirmEdit
}

func event(_ eventName: EventName) -> String {
    return eventName.rawValue
}

struct ConfirmEditRequest: Decodable {
    let originalName: String
    let trackInfo: TrackInfo
}

enum ConfirmEditResponse: Encodable {
    enum CodingKeys: String, CodingKey {
        case resolution, newName
    }
    
    case allow
    case deny
    case rename(newName: String)
    
    func encode(to encoder: Encoder) throws {
        var container = encoder.container(keyedBy: CodingKeys.self)
        switch self {
        case .allow:
            try container.encode("allow", forKey: .resolution)
        case .deny:
            try container.encode("deny", forKey: .resolution)
        case .rename(let newName):
            try container.encode("rename", forKey: .resolution)
            try container.encode(newName, forKey: .newName)
            
        }
    }
}
