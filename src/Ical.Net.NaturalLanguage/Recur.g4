grammar Recur;

file : recur specat? count? EOF ;

recur 
    : EVERY interval specon?
    | EVERY monthlist specon?
    | EVERY daylist
    | EVERY WEEKDAY
    ;

count : FOR {CurrentToken.Text!="0"}? NUMBER TIMES ;

monthlist
    : MONTHNAME ((AND | ',') MONTHNAME)?
    | MONTHNAME (',' MONTHNAME)+ AND MONTHNAME
    ;

daylist
    : DAYOFWEEK ((AND | ',') DAYOFWEEK)?
    | DAYOFWEEK (',' DAYOFWEEK)+ AND DAYOFWEEK
    ;

interval
    : SFREQ                                             #interval_plain
    | {IsTwoOrMore(CurrentToken.Text)}? NUMBER PFREQ    #interval_plural
    | ORDINAL? DAYOFWEEK                                #interval_ordinal_dayofweek
    | ORDINAL? MONTHNAME                                #interval_ordinal_month
    ;

specon
    : ON THE dayofmonthlist     #spec_onmonthday
    | ON dayofweeklist          #spec_onweekday
    ;

specat
    : AT timespec
    ;

timespec
    : NUMBER ((AND | ',') NUMBER)?
    | NUMBER (',' NUMBER)+ AND NUMBER
    ;

dayofmonthlist
    : dayofmonthspec ((AND | ',') dayofmonthspec)?
    | dayofmonthspec (',' dayofmonthspec)+ AND dayofmonthspec
    ;

dayofmonthspec
    : ORDINAL LAST?
    ;

dayofweeklist
    : dayofweekspec ((AND | ',') dayofweekspec)?
    | dayofweekspec (',' dayofweekspec)+ AND dayofweekspec
    ;

dayofweekspec
    : DAYOFWEEK                     #dayofweekspec_plain
    | THE ORDINAL DAYOFWEEK         #dayofweekspec_ordinal
    | THE ORDINAL? LAST DAYOFWEEK   #dayofweekspec_last_ordinal
    ;

FOR : 'for' ;

LAST : 'last' ;

TIMES : 'times' ;

AND : 'and' ;

THE : 'the' ;

AT : 'at' ;

ON : 'on' ;

EVERY : 'every' ;

WEEKDAY : 'weekday' ;

SFREQ : 'day' | 'week' | 'month' | 'year' | 'hour';

PFREQ : 'days' | 'weeks' | 'months' | 'years' | 'hours';

DAYOFWEEK : 'sunday' | 'monday' | 'tuesday' | 'wednesday' | 'thursday' | 'friday' | 'saturday'
    | 'sun' | 'mon' | 'tue' | 'wed' | 'thu' | 'fri' | 'sat' ;

MONTHNAME : 'january' | 'february' | 'march' | 'april' | 'may' | 'june' | 'july' | 'august' | 'september' | 'october' | 'november' | 'december'
    | 'jan' | 'feb' | 'mar' | 'apr' | 'jun' | 'jul' | 'aug' | 'sep' | 'oct' | 'nov' | 'dec' ;

NUMBER : '0' | [1-9][0-9]* ;

ORDINAL : NUMBER? ORDINALFRAG ;

fragment ORDINALFRAG : '1st' | '2nd' | '3rd' | [4-9] 'th' | '11th' | '12th' | '13th' ;

Whitespace
    :   [ \t]+
        -> skip
    ;

Newline
    :   (   '\r' '\n'?
        |   '\n'
        )
        -> skip
    ;
