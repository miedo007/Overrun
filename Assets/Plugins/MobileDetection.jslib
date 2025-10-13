mergeInto(LibraryManager.library, {
    DetectMobile: function() {
        // Get user agent string
        var userAgent = navigator.userAgent || navigator.vendor || window.opera;
        
        // Check for mobile device patterns
        var isMobile = /android|iphone|ipad|ipod|blackberry|iemobile|opera mini/i.test(userAgent);
        
        // Also check for touch capability as secondary validation
        var hasTouch = 'ontouchstart' in window || navigator.maxTouchPoints > 0;
        
        // Log detection info for debugging (visible in browser console)
        console.log('[MobileDetection] User Agent: ' + userAgent);
        console.log('[MobileDetection] Mobile pattern match: ' + isMobile);
        console.log('[MobileDetection] Has touch: ' + hasTouch);
        console.log('[MobileDetection] Final result: ' + (isMobile ? 'Mobile' : 'Desktop'));
        
        // Send result back to Unity GameObject named 'HintManager'
        SendMessage('HintManager', 'OnMobileDetected', isMobile ? 'true' : 'false');
    }
});
