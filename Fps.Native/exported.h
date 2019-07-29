//
// Created by thomas on 7/23/19.
//

#ifndef FPSNATIVE_EXPORTED_H
#define FPSNATIVE_EXPORTED_H

// exported.h
#pragma once

// Define EXPORTED for any platform
#ifdef _WIN32
# ifdef WIN_EXPORT
#   define EXPORTED  __declspec( dllexport )
# else
#   define EXPORTED  __declspec( dllimport )
# endif
#else
# define EXPORTED
#endif

#endif //FPSNATIVE_EXPORTED_H
