## Title
Logout button unresponsive in Safari browser

## Description
The logout functionality is not working properly in Safari browser. When users attempt to log out of the application, the logout button does not respond to clicks, preventing users from properly ending their session. This creates a security concern as users cannot reliably terminate their authenticated sessions.

## Steps to Reproduce
1. Open the application in Safari browser
2. Log in with valid user credentials
3. Navigate to the page containing the logout button
4. Click on the logout button
5. Observe that no action occurs

## Expected vs Actual Behavior
**Expected:** User should be logged out of the application and redirected to login page or homepage
**Actual:** Logout button does not respond to clicks, user remains logged in

## Environment
**Browser:** Safari [NEEDS VERIFICATION - latest version assumed]
**Operating System:** [NEEDS VERIFICATION - macOS/iOS assumed]
**Application Version:** [NEEDS VERIFICATION]
**Device:** [NEEDS VERIFICATION - Desktop/Mobile]

## Severity/Impact
**Medium** - While this doesn't prevent core functionality, it affects user security by preventing proper session termination. Users may need to clear browser data or close browser entirely to log out, which impacts user experience and creates potential security risks.