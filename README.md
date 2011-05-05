qtools
======

`qtools` is an MSMQ administration and operation toolkit.  

With `qtools` you'll be able to perform both deployment and ongoing operation actions pretty easily -- modeled 

after the [UNIX phylosophy](http://en.wikipedia.org/wiki/Unix_philosophy):

__Write programs that do one thing and do it well. Write programs to work together. Write programs to handle 

text streams, because that is a universal interface__

In practice the `qtools` are *mostly* conforming to that model.


The Tools
---------

* `qls` - list queues. *This is an enabler of pipe operations* when you want to run batch commands.
* `qcount` - count messages in a queue.
* `qcp` - copy queue content to another queue.
* `qgrep` - grep-like tool for searching inside queue messages.
* `qrm` - remove a queue
* `qtail` - tail a queue (show contents and a live message feed)
* `qtouch` - create a queue
* `qtruncate` - truncate (empty a queue)

Most of the tools try to rely on their UNIX counterparts for name semantics.  

An input is a queue path. If `-n` isn't required and you dont specify a queue path via `-n` you'll be prompted 

for `STDIN`.

From what you'll see below, you might be finally able to throw away all of those pesky `vbs`, `bat`, and `powershell` scripts 

:).


Quick Examples
--------------

Some of the things you can do with the tools as a collection or separately:


Creating a new *transactional* queue with *full permissions* for *Everyone*, a *limit of 400KB*. Not all 

parameters are required.  

    qtouch -n .\private$\foo_q -p FullControl -u Everyone -l 400 -t



Creating a set of queues from a text file (as part of deployment for example). Note that I use MSYS/Mingw's `cat` to stream the text out.  

    # queues.txt  --snip-snip--
    .\private$\xmltestqueue
    .\private$\xmltestqueue


    $ cat queues.txt | qtouch -p FullControl -u Everyone -l 400 -t




Counting number of messages in a single queue  

    $ qcount -n .\private$\xmltestqueue
    OK: [.\private$\xmltestqueue] 3 message(s).



Counting number of messages in a list of queues (using `qls` with a pipe)  

    $ qls -f xmltest | qcount
    OK: [.\private$\xmltestqueue] 3 message(s).
    OK: [.\private$\xmltestqueue2] 0 message(s).

Removing, truncating and such in the same fasion (use `qrm`, `qtruncate` instead of `qcount`)  

Grepping queue contents can be fun (you can also use `qls` to grep on several queues!):

    $ qgrep.exe -n .\private$\xmltestqueue -e a
    INFO: [.\private$\xmltestqueue] Listing results.
    5/3/2011 7:11:40 PM     *** message id ***    foo:    f<a>sdf
    5/3/2011 7:21:26 PM     *** message id ***    asdfadf:        <a>ef


Tailing a queue is also fun - you should see messages on the terminal as they're added to the queue:

    $ qtail -n .\private$\foo_q


Even More
---------
You *should* check out each command with its various switches. I've only covered a small subset of what you could do with `qtools`.


Output
------
The `qtools` output was adjusted to be easily parsable by regex or simple matchers (by position and tabs) so 

that if needed, it can be piped to a monitoring or logging system.





