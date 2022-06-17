# Recyclable Item Container
Heavily inspired and based on [Recyclable Scroll Rect](https://github.com/MdIqubal/Recyclable-Scroll-Rect) which got problems I couldn't live with:
- if dataSource have 0 items - it would fail to inilitize after 
- if dataSource is modified - RecyclableScrollRect doesn't know about that and does not reload.
  - if dataSource started from 0 elements - reload didn't help (for me)
  - if dataSource is modified very frequently (eg in Update) - it performs poorly
- does not work with layout groups

So I did this package.
- It uses LinkedList for shown items.
- ScrollRectExtended is just one new method with offset on drag after recycle
- Better handling of recycle logic - inheritance rules! 

# Installing
Please reffer to [Package Manager installation instructions](https://docs.unity3d.com/Manual/upm-ui-install.html)

# Sample
You can download sample in package manager directly from this package

# Known bugs
- In the sample (when I was testing generating data in Update) it can happen that list uppears to reach it's end, but it crearly did not (as I generate more data). It happend because of small max active elements and randomness of elements' sizes - if it's too many short in a row - it can stop recycling in both ways. It could help to increase max elements, using bigger recycling threshold or increasing elasticity of scroll rect.

# Todo
- Scrollbar
- Scroll to bottom/top or specific item
