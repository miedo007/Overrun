mergeInto(LibraryManager.library, {
    DetectMobile: function() {
        // Get user agent string
        var userAgent = navigator.userAgent || navigator.vendor || window.opera;
        
        // Standard mobile detection
        var isMobile = /android|iphone|ipad|ipod|blackberry|iemobile|opera mini/i.test(userAgent);
        
        // Special iPad detection for modern iPads that masquerade as desktop
        var isIPadMasquerading = false;
        if (!isMobile) {
            // Check if it's an iPad pretending to be a Mac
            var isLikelyMac = /macintosh/i.test(userAgent);
            var hasTouch = 'ontouchstart' in window || navigator.maxTouchPoints > 0;
            var screenWidth = screen.width;
            var screenHeight = screen.height;
            
            // iPad characteristics: touch + reasonable tablet size + Mac user agent
            var hasTabletScreenSize = (screenWidth >= 768 && screenWidth <= 1366) || 
                                      (screenHeight >= 768 && screenHeight <= 1366);
            
            isIPadMasquerading = isLikelyMac && hasTouch && hasTabletScreenSize;
        }
        
        // Final mobile decision
        var finalIsMobile = isMobile || isIPadMasquerading;
        
        // Enhanced logging for debugging
        console.log('[MobileDetection] User Agent: ' + userAgent);
        console.log('[MobileDetection] Standard mobile match: ' + isMobile);
        console.log('[MobileDetection] iPad masquerading check: ' + isIPadMasquerading);
        console.log('[MobileDetection] Screen: ' + screen.width + 'x' + screen.height);
        console.log('[MobileDetection] Has touch: ' + ('ontouchstart' in window || navigator.maxTouchPoints > 0));
        console.log('[MobileDetection] Final result: ' + (finalIsMobile ? 'Mobile' : 'Desktop'));
        
        // Send result back to Unity
        SendMessage('HintManager', 'OnMobileDetected', finalIsMobile ? 'true' : 'false');
    }
});
